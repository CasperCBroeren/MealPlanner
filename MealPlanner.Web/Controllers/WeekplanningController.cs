using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{

    [Route("api/[controller]"), 
        Authorize(Policy = "GroupOnly")]
    public class WeekplanningController : BaseController
    {
        private IWeekplanningRepository weekplanning;

        public WeekplanningController(IWeekplanningRepository weekplanningRepository)  
        {
            this.weekplanning = weekplanningRepository;
        } 

        [HttpGet("{year}/{week}")]
        public async Task<IActionResult> Current(int year, int week)
        {
            var item = await this.weekplanning.GetForWeekAndYear(year, week,  this.GroupId().Value);
            return Ok(item);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Save([FromBody]Weekplanning plan)
        {
            var item = await this.weekplanning.Save(plan, this.GroupId().Value);
            return Ok(item);
        } 
    }
}
