using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{

    [Route("api/[controller]")]
    public class WeekplanningController : Controller
    {
        private IWeekplanningRepository weekplanning;

        public WeekplanningController(IWeekplanningRepository weekplanningRepository)
        {
            this.weekplanning = weekplanningRepository;
        } 

        [HttpGet("{year}/{week}")]
        public async Task<IActionResult> Current(int year, int week)
        {
            var item = await this.weekplanning.GetForWeekAndYear(year, week);
            return Ok(item);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Save(Weekplanning plan)
        {
            var item = await this.weekplanning.Save(plan);
            return Ok(item);
        }



    }
}
