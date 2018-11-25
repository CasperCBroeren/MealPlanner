using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using MealPlanner.Web.ViewModels;

namespace mealplanner.Controllers
{
    [Route("api/[controller]")]
    public class MealController : Controller
    {
        private IMealRepository mealRepository;

        public MealController(IMealRepository mealRepository)
        {
            this.mealRepository = mealRepository;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> All()
        {
            var items = await this.mealRepository.All();
            return Ok(items); 
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var items = await this.mealRepository.FindOneById(id);
            return Ok(items);
        }


        [HttpPost("[action]")]
        public async  Task<ActionResult> FindByIngredients([FromBody] Ingredient[] ingredients)
        {
            var item = await this.mealRepository.FindByIngredients(ingredients);
            if (item != null)
            {
                return Ok(item);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> FindByTagsandType([FromBody] SearchByTagsAndType options)
        {

            var item = await this.mealRepository.FindByTagAndType(options.Tags, options.Type);
            if (item != null)
            {
                return Ok(item);
            }
            else
            {
                return NotFound();
            }
        }
          
        [HttpGet("[action]/{term}")]
        public async Task<ActionResult> Find([FromRoute]string term)
        {
            var item = await this.mealRepository.FindAllByTerm(term);
            if (item != null)
            {
                return Ok(item);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Save([FromBody] Meal item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var isDuplicate = false;
                if (!item.Created.HasValue)
                { 
                    var existingItem = await this.mealRepository.FindOneByName(item.Name);
                    if (existingItem == null)
                    {
                        item.Created = DateTime.Now;
                    }
                    else 
                    {
                        isDuplicate = true;
                        item.Created = existingItem.Created;
                    }
                }

                var result = await this.mealRepository.Save(item);
                return Ok( new {
                                created = item.Created,
                                isDuplicate = isDuplicate
                                });
            }
            return Ok("nope");
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> Delete([FromBody] Meal item)
        {
            var result = await this.mealRepository.Delete(item);
            return Ok(result ? "done": "nochange");
        }
    }

}
