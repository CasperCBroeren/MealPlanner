using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{
    [Route("api/[controller]"),
        Authorize(Policy = "GroupOnly")]
    public class ShoppingListController : BaseController
    {
        private readonly IBoughtIngredientRepository boughtIngredientRepository;

        public ShoppingListController(IBoughtIngredientRepository boughtIngredientRepository)  
        {
            this.boughtIngredientRepository = boughtIngredientRepository;
        }

        [HttpGet("[action]/{year}/{week}")]
        public async Task<ActionResult> Get(int year, int week)
        {
            var items = await this.boughtIngredientRepository.GetForWeekAndYear(year, week, this.GroupId().Value);
            return Ok(items);
        } 

        [HttpPost("[action]")]
        public async Task<ActionResult> Save([FromBody] BoughtIngredient item)
        { 
            var existingItem = await this.boughtIngredientRepository.Save(item, this.GroupId().Value); 
            return Ok();
        } 
    } 
}
