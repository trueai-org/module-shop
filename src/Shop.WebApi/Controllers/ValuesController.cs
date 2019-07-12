using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Module.Schedule.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger _logger;

        public ValuesController(IJobService jobService, ILogger<ValuesController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            _logger.LogInformation("now:" + DateTime.Now.ToLongTimeString());
            await _jobService.Schedule(() => Log(), TimeSpan.FromSeconds(5));
            return new string[] { "value1", "value2" };
        }

        public async Task Log()
        {
            _logger.LogInformation("after:" + DateTime.Now.ToLongTimeString());
            await Task.CompletedTask;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
