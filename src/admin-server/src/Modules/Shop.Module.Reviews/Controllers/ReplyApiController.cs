using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Abstractions.Extensions;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.MQ.Abstractions.Data;
using Shop.Module.MQ.Abstractions.Services;
using Shop.Module.Reviews.Abstractions.Data;
using Shop.Module.Reviews.Abstractions.Entities;
using Shop.Module.Reviews.Abstractions.Events;
using Shop.Module.Reviews.Abstractions.Models;
using Shop.Module.Reviews.Abstractions.Services;
using Shop.Module.Reviews.Abstractions.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Reviews.Controllers
{
    [Route("api/replies")]
    [Authorize()]
    public class ReplyApiController : ControllerBase
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Support> _supportRepository;
        private readonly IRepository<Reply> _replyRepository;
        private readonly IWorkContext _workContext;
        private readonly IMQService _mqService;
        private readonly IAppSettingService _appSettingService;

        public ReplyApiController(
            IRepository<Review> reviewRepository,
            IRepository<Support> supportRepository,
            IRepository<Reply> replyRepository,
            IWorkContext workContext,
            IMQService mqService,
            IAppSettingService appSettingService)
        {
            _reviewRepository = reviewRepository;
            _supportRepository = supportRepository;
            _replyRepository = replyRepository;
            _workContext = workContext;
            _mqService = mqService;
            _appSettingService = appSettingService;
        }

        [HttpPost()]
        public async Task<Result> Post([FromBody]ReplyAddParam param)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var reply = new Reply
            {
                ReviewId = param.ReviewId,
                Comment = param.Comment,
                IsAnonymous = param.IsAnonymous,
                ParentId = null,
                UserId = user.Id,
                ReplierName = param.IsAnonymous ? $"{user.FullName.First()}***{user.FullName.Last()}" : user.FullName,
            };

            if (param.ToReplyId != null)
            {
                var toReply = await _replyRepository.FirstOrDefaultAsync(param.ToReplyId.Value);
                if (toReply == null)
                {
                    throw new Exception("回复信息不存在");
                }
                reply.ToUserId = toReply.UserId;
                reply.ToUserName = toReply.ReplierName;
                reply.ParentId = toReply.ParentId ?? toReply.Id;
            }
            _replyRepository.Add(reply);
            await _replyRepository.SaveChangesAsync();

            var isAuto = await _appSettingService.Get<bool>(ReviewKeys.IsReplyAutoApproved);
            if (isAuto)
            {
                await _mqService.Send(QueueKeys.ReplyAutoApproved, new ReplyAutoApprovedEvent()
                {
                    ReplyId = reply.Id
                });
            }
            return Result.Ok();
        }

        [HttpPost("grid")]
        [AllowAnonymous]
        public async Task<Result<StandardTableResult<ReplyListResult>>> Grid([FromBody]StandardTableParam<ReplyQueryParam> param)
        {
            var search = param?.Search;
            if (search == null)
                throw new ArgumentNullException("参数异常");

            var query = _replyRepository.Query()
               .Where(c => c.Status == ReplyStatus.Approved && c.ParentId == null && c.ReviewId == search.ReviewId);

            var result = await query
                .Include(c => c.User)
                .Include(c => c.Childrens).ThenInclude(c => c.ToUser)
                .ToStandardTableResult(param, c => new ReplyListResult
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    CreatedOn = c.CreatedOn,
                    SupportCount = c.SupportCount,
                    Avatar = c.User.AvatarUrl,
                    ReplierName = c.ReplierName,
                    Replies = c.Childrens.Where(x => x.Status == ReplyStatus.Approved).OrderByDescending(x => x.Id).Select(x => new ReplyListResult()
                    {
                        Id = x.Id,
                        Comment = x.Comment,
                        ReplierName = x.ReplierName,
                        CreatedOn = x.CreatedOn,
                        SupportCount = x.SupportCount,
                        ToUserName = x.ToUserName
                    })
                });
            return Result.Ok(result);
        }
    }
}
