using System.Collections.Generic;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// This is a test API controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Get Values
        /// </summary>
        /// <returns></returns>
        // GET: api/Home
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Accessed Home Controller");
            return new string[] {"value1", "value2"};
        }
        
        /// <summary>
        /// Get app value
        /// </summary>
        /// <param name="id">ID of thing to get</param>
        /// <returns>Some info about the ID</returns>
        // Get: api/Home/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            _logger.LogDebug("Got a value");
            return "value";
        }
        
        // POST: api/Home
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("Nothing to POST!");
        }
        
        // PUT: api/Home/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("You tried to delete something!");
        }
    }
}