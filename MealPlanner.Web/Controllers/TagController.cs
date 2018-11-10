using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{

    [Route("api/[controller]")]
    public class TagController : Controller
    {
        private ITagRepository tagRepository;

        public TagController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet("[action]/{tag}")]
        public async Task<IActionResult> Find(string tag)
        {
            var items = await this.tagRepository.Find(tag);
            return Ok(items);
        }

        [HttpGet("[action]/{startWith}")]
        public async Task<IActionResult> Search(string startWith)
        {
            var items = await this.tagRepository.FindStartingWith(startWith);
            return Ok(items);
        }
    }
}
