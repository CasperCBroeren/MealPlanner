using System;  
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Data.Models; 
using MealPlanner.Data.Repositories;

namespace mealplanner.Controllers
{
    [Route("api/[controller]")]
    public class IngredientsController : Controller
    {
        private IIngredientRepository ingredientRepository;

        public IngredientsController(IIngredientRepository ingredientRepository)
        {
            this.ingredientRepository = ingredientRepository;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> All()
        {
            var items = await this.ingredientRepository.All();
            return Ok(items); 
        }


        [HttpGet("[action]/{name}")]
        public async Task<ActionResult> Find([FromRoute]string name)
        {
            var item = await this.ingredientRepository.FindSingleByName(name);
            if (item !=null ) 
            {
                return Ok(item);
            } 
            else 
            {
                return NotFound();
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Save([FromBody] Ingredient item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var isDuplicate = false;
                if (!item.Uid.HasValue)
                { 
                    var existingItem = await this.ingredientRepository.FindSingleByName(item.Name);
                    if (existingItem == null)
                    {
                        item.Uid = Guid.NewGuid();
                    }
                    else 
                    {
                        isDuplicate = true;
                        item.Uid = existingItem.Uid;
                    }
                }

                var result = await this.ingredientRepository.Save(item);
                return Ok( new {
                                uid = item.Uid,
                                isDuplicate = isDuplicate,
                                item = item
                                });
            }
            return Ok("nope");
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> Delete([FromBody] Ingredient item)
        {
            var result = await this.ingredientRepository.Delete(item);
            return Ok(result ? "done": "nochange");
        }
    }

}
