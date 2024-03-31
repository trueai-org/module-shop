using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Extensions;
using Shop.Module.MQ;
using Shop.Module.Reviews.Entities;
using Shop.Module.Reviews.Services;
using Shop.Module.Reviews.ViewModels;

namespace Shop.Module.Reviews.Controllers
{
    /// <summary>
    /// 管理员回复 API 控制器，用于处理管理员对评论的回复操作。
    /// </summary>
    [Authorize(Roles = "admin")]
    [Route("api/admin-replies")]
    public class AdminReplyApiController : ControllerBase
    {
        private readonly IRepository<Review> _reviewRepository;
        private readonly IRepository<Support> _supportRepository;
        private readonly IRepository<Reply> _replyRepository;
        private readonly IWorkContext _workContext;
        private readonly IMQService _mqService;

        public AdminReplyApiController(
            IRepository<Review> reviewRepository,
            IRepository<Support> supportRepository,
            IRepository<Reply> replyRepository,
            IWorkContext workContext,
            IMQService mqService)
        {
            _reviewRepository = reviewRepository;
            _supportRepository = supportRepository;
            _replyRepository = replyRepository;
            _workContext = workContext;
            _mqService = mqService;
        }

        /// <summary>
        /// 分页获取管理员回复列表。
        /// </summary>
        /// <param name="param">分页和筛选参数。</param>
        /// <returns>管理员回复的分页列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<AdminReplyListResult>>> Grid([FromBody] StandardTableParam<AdminReplyQueryParam> param)
        {
            var query = _replyRepository.Query();
            var search = param?.Search;
            if (search != null)
            {
                if (search.Status.HasValue)
                {
                    query = query.Where(c => c.Status == search.Status.Value);
                }

                if (!string.IsNullOrWhiteSpace(search.ReplierName))
                {
                    query = query.Where(c => c.ReplierName.Contains(search.ReplierName));
                }
            }
            var result = await query
                .ToStandardTableResult(param, c => new AdminReplyListResult
                {
                    Id = c.Id,
                    Comment = c.Comment,
                    CreatedOn = c.CreatedOn,
                    SupportCount = c.SupportCount,
                    ReplierName = c.ReplierName,
                    IsAnonymous = c.IsAnonymous,
                    ParentId = c.ParentId,
                    ReviewId = c.ReviewId,
                    Status = c.Status,
                    ToUserId = c.ToUserId,
                    ToUserName = c.ToUserName,
                    UpdatedOn = c.UpdatedOn,
                    UserId = c.UserId
                });
            return Result.Ok(result);
        }

        /// <summary>
        /// 更新指定 ID 的管理员回复的状态。
        /// </summary>
        /// <param name="id">管理员回复 ID。</param>
        /// <param name="param">回复更新参数。</param>
        /// <returns>更新操作的结果。</returns>
        [HttpPut("{id}")]
        public async Task<Result> Put(int id, [FromBody] AdminReplyUpdateParam param)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _replyRepository.FirstOrDefaultAsync(id);
            if (model != null)
            {
                model.Status = param.Status;
                model.UpdatedOn = DateTime.Now;
                await _replyRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }

        /// <summary>
        /// 删除指定 ID 的管理员回复。
        /// </summary>
        /// <param name="id">管理员回复 ID。</param>
        /// <returns>删除操作的结果。</returns>
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            var user = await _workContext.GetCurrentOrThrowAsync();
            var model = await _replyRepository.FirstOrDefaultAsync(id);
            if (model != null)
            {
                var any = _replyRepository.Query().Any(c => c.ParentId == model.Id);
                if (any)
                {
                    throw new Exception("当前回复下存在子回复，不允许删除");
                }

                model.IsDeleted = true;
                model.UpdatedOn = DateTime.Now;
                await _replyRepository.SaveChangesAsync();
            }
            return Result.Ok();
        }
    }
}