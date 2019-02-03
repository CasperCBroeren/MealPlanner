
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace MealPlanner.Web.Controllers
{
    [Route("api/[controller]"),
        Authorize(Policy = "GroupOnly")] 
    public class IngredientsController : BaseController
    {
        private IIngredientRepository ingredientRepository;

        public IngredientsController(IGroupRepository groupRepository, IIngredientRepository ingredientRepository)
        {
            this.ingredientRepository = ingredientRepository;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> All()
        {
            var items = await this.ingredientRepository.All(await this.GroupId());
            return Ok(items); 
        }

        [HttpGet("[action]/{part}")]
        public async Task<ActionResult> Search([FromRoute]string part)
        {
            var items = await this.ingredientRepository.SearchByPart(await this.GroupId(), part);
            if (items != null)
            {
                return Ok(items);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("[action]/{name}")]
        public async Task<ActionResult> Find([FromRoute]string name)
        {
            var item = await this.ingredientRepository.FindSingleByName(await this.GroupId(), name);
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
                var result = await this.ingredientRepository.Save(await this.GroupId(),item);
                return Ok( new {
                                id = item.Id, 
                                item = item
                                });
            }
            return Ok("nope");
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> Delete([FromBody] Ingredient item)
        {
            var result = await this.ingredientRepository.Delete(await this.GroupId(),item);
            return Ok(result ? "done": "nochange");
        }
    }

}
