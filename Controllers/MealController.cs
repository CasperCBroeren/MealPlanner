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
    public class MealController : Controller
    {  
        [HttpGet("[action]")]
        public async Task<ActionResult> All()
        {    
            var items = await Client.Instance.SearchAsync<Meal>(s => s.MatchAll().Index(Client.MealIndexName));
            return Ok(items.Documents.ToList()); 
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Save([FromBody] Meal item)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                var isDuplicate = false;
                if (!item.Created.HasValue)
                {
                    var checkExisting = await Client.Instance.SearchAsync<Meal>(q => q.Query(rq => rq.Term(t => t.ExactName, item.Name)).Index(Client.MealIndexName));
                    var existingItem = checkExisting.Documents.FirstOrDefault();
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
                
                var result = await Client.Instance.IndexDocumentAsync(item);
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
            var result = await Client.Instance.DeleteByQueryAsync<Meal>(q => q.Query(rq => rq.Term(t => t.Created, item.Created)));
            return Ok(result.Deleted > 0 ? "done": "nochange");
        }
    }

}