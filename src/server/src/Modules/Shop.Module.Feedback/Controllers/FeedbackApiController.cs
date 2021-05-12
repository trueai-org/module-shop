using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Extensions;
using Shop.Module.Feedbacks.Entities;
using Shop.Module.Feedbacks.ViewModels;
using System.Threading.Tasks;

namespace Shop.Module.Feedbacks.Controllers
{
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

        [HttpPost()]
        public async Task<Result> Post([FromBody]FeedbackAddParam param)
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
