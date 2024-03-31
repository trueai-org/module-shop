using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Feedbacks.Entities;
using Shop.Module.Feedbacks.ViewModels;

namespace Shop.Module.Feedbacks.Controllers
{
    /// <summary>
    /// 反馈API控制器，用于处理用户反馈相关的请求
    /// </summary>
    [Route("api/feedbacks")]
    public class FeedbackApiController : ControllerBase
    {
        private readonly IRepository<Feedback> _feedbackRepository;
        private readonly IWorkContext _workContext;

        public FeedbackApiController(
            IRepository<Feedback> feedbackRepository,
            IWorkContext workContext)
        {
            _feedbackRepository = feedbackRepository;
            _workContext = workContext;
        }

        /// <summary>
        /// 接收并保存用户反馈信息
        /// </summary>
        /// <param name="param">用户反馈添加参数，包含联系方式、内容和类型</param>
        /// <returns>返回操作结果，表示反馈信息是否成功保存</returns>
        [HttpPost()]
        public async Task<Result> Post([FromBody] FeedbackAddParam param)
        {
            var user = await _workContext.GetCurrentUserOrNullAsync();
            var model = new Feedback()
            {
                UserId = user?.Id,
                Contact = param.Contact,
                Content = param.Content,
                Type = param.Type.Value
            };
            _feedbackRepository.Add(model);
            await _feedbackRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}