using Microsoft.AspNetCore.Mvc;
using OnlineChatBackend.Interfaces;

namespace OnlineChatBackend.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly IWebSearchService _search;

        public SearchController(IWebSearchService search)
        {
            _search = search;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var results = await _search.SearchAsync(q);
            return Ok(results);
        }
    }
}
