using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace mealplanner.Controllers
{
    [Route("api/[controller]")]
    public class ShoppingListController : Controller
    {
        private readonly IBoughtIngredientRepository boughtIngredientRepository;

        public ShoppingListController(IBoughtIngredientRepository boughtIngredientRepository)
        {
            this.boughtIngredientRepository = boughtIngredientRepository;
        }

        [HttpGet("[action]/{year}/{week}")]
        public async Task<ActionResult> Get(int year, int week)
        {
            var items = await this.boughtIngredientRepository.GetForWeekAndYear(year, week);
            return Ok(items);
        } 

        [HttpPost("[action]")]
        public async Task<ActionResult> Save([FromBody] BoughtIngredient item)
        { 
            var existingItem = await this.boughtIngredientRepository.Save(item); 
            return Ok();
        } 
    } 
}
