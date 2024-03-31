using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Services;
using Shop.Module.MQ;
using Shop.Module.Reviews.Data;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Events;
using Shop.Module.Reviews.Models;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers
{
    /// <summary>
    /// 评论回复 API 控制器，用于处理评论的回复操作。
    /// </summary>
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


        /// <summary>
        /// 发布一条评论回复。
        /// </summary>
        /// <param name="param">评论回复的参数。</param>
        /// <returns>操作的结果。</returns>
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

        /// <summary>
        /// 分页获取指定评论的所有通过审核的回复。
        /// </summary>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>指定评论的回复列表。</returns>
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
