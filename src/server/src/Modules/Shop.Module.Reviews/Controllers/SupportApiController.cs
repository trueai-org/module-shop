using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers
{
    /// <summary>
    /// 点赞 API 控制器，负责处理点赞相关操作。
    /// </summary>
    [Route("api/supports")]
    [Authorize()]
    public class SupportApiController : ControllerBase
    {
        private readonly EntityTypeWithId[] supportEntityTypeIds = new EntityTypeWithId[] { EntityTypeWithId.Review, EntityTypeWithId.Reply };

        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Support> _supportRepository;
        private readonly IRepository<Reply> _replyRepository;
        private readonly IWorkContext _workContext;

        public SupportApiController(
            IRepository<Review> reviewRepository,
            IRepository<Support> supportRepository,
            IRepository<Reply> replyRepository,
            IWorkContext workContext)
        {
            _reviewRepository = reviewRepository;
            _supportRepository = supportRepository;
            _replyRepository = replyRepository;
            _workContext = workContext;
        }

        /// <summary>
        /// 赞(支持)/取消赞
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result> Support([FromBody] SupportParam param)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var any = supportEntityTypeIds.Any(c => c == param.EntityTypeId);
            if (!any)
                throw new Exception("参数不支持");

            var model = await _supportRepository
                .Query(c => c.UserId == user.Id && c.EntityId == param.EntityId && c.EntityTypeId == (int)param.EntityTypeId)
                .FirstOrDefaultAsync();
            if (model == null)
            {
                model = new Support()
                {
                    UserId = user.Id,
                    EntityId = param.EntityId,
                    EntityTypeId = (int)param.EntityTypeId
                };
                _supportRepository.Add(model);
            }
            else
            {
                model.IsDeleted = true;
                model.UpdatedOn = DateTime.Now;
            }

            var supportCount = 0;
            switch (param.EntityTypeId)
            {
                case EntityTypeWithId.Review:
                    {
                        var review = await _reviewRepository.FirstOrDefaultAsync(param.EntityId);
                        if (review == null)
                            throw new Exception("评论信息不存在");
                        review.SupportCount += model.IsDeleted ? -1 : 1;
                        review.UpdatedOn = DateTime.Now;
                        supportCount = review.SupportCount;
                    }
                    break;

                case EntityTypeWithId.Reply:
                    {
                        var reply = await _replyRepository.FirstOrDefaultAsync(param.EntityId);
                        if (reply == null)
                            throw new Exception("评论信息不存在");
                        reply.SupportCount += model.IsDeleted ? -1 : 1;
                        reply.UpdatedOn = DateTime.Now;
                        supportCount = reply.SupportCount;
                    }
                    break;

                default:
                    throw new Exception("参数不支持");
            }

            using (var tran = _supportRepository.BeginTransaction())
            {
                await _reviewRepository.SaveChangesAsync();
                await _replyRepository.SaveChangesAsync();
                await _supportRepository.SaveChangesAsync();
                tran.Commit();
            }
            return Result.Ok(supportCount);
        }
    }
}