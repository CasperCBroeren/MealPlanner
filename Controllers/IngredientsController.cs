using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mealplanner.Models;
using Nest; 
using Client = ElasticEndpoint;

namespace mealplanner.Controllers
{
    [Route("api/[controller]")]
    public class IngredientsController : Controller
    {  
        [HttpGet("[action]")]
        public async Task<ActionResult> All()
        {    
            var items = await Client.Instance.SearchAsync<Ingredient>(s => 
                                                                        s.MatchAll()
                                                                        .Index(Client.IngredientIndexName)
                                                                        .Sort(ss => ss.Ascending(p => p.ExactName)));
            return Ok(items.Documents.ToList()); 
        }


        [HttpGet("[action]/{name}")]
        public async Task<ActionResult> Find([FromRoute]string name)
        {
            var item = await Client.Instance.SearchAsync<Ingredient>(s => s.Query(q => q.Term(t => t.ExactName, name)).Index(Client.IngredientIndexName));
            if (item.Documents.Count == 1) 
            {
                return Ok(item.Documents.First());
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
                    var checkExisting = await Client.Instance.SearchAsync<Ingredient>(q => q.Query(rq => rq.Term(t => t.ExactName, item.Name)).Index(Client.IngredientIndexName));
                    var existingItem = checkExisting.Documents.FirstOrDefault();
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
                
                var result = await Client.Instance.IndexDocumentAsync<Ingredient>(item);
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
            var result = await Client.Instance.DeleteByQueryAsync<Ingredient>(q => q.Query(rq => rq.Term(t => t.Uid, item.Uid)));
            return Ok(result.Deleted > 0 ? "done": "nochange");
        }
    }

}