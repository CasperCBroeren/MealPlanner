using Elasticsearch.Net;
using MealPlanner.Data.Elastic;
using MealPlanner.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories.Elastic
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ElasticService elasticService;

        public IngredientRepository(ElasticService elasticService)
        {
            this.elasticService = elasticService;
        }
        public async Task<IEnumerable<Ingredient>> All()
        {
            var result = await elasticService.Client.SearchAsync<Ingredient>(s =>
                                                                 s.MatchAll()
                                                                 .Index(ElasticService.IngredientIndexName)
                                                                 .Sort(ss => ss.Ascending(p => p.Name)));
            return result.Documents.ToList();
        }

        public async Task<bool> Delete(Ingredient item)
        {
            var result = await this.elasticService.Client.DeleteByQueryAsync<Ingredient>(q => q.Query(rq => rq.Term(t => t.Id, item.Id)));
            return result.Deleted > 0;
        }

        public async Task<IEnumerable<Ingredient>> FindAllByName(string name)
        {
            var item = await this.elasticService.Client.SearchAsync<Ingredient>(s => s.Suggest(q => q.Term("name_suggester",
                t => t 
                    .Analyzer("standard")
                    .Field(p => p.Name) 
                    .Text(name)
                )).Index(ElasticService.IngredientIndexName));
            return item.Documents.ToList();
        }

        public async Task<Ingredient> FindSingleByName(string name)
        {
            var item = await this.elasticService.Client.SearchAsync<Ingredient>(s => s.Query(q => q.Term(t => t.Name, name)).Index(ElasticService.IngredientIndexName));
            return item.Documents.Count > 0 ? item.Documents.First() : null;
        }

        public async Task<bool> Save(Ingredient item)
        {
            if (!item.Id.HasValue)
            {
                var existingItem = await this.FindSingleByName(item.Name);
                if (existingItem == null)
                {
                    item.Id = System.Environment.TickCount;
                }
                
            }
            var result = await this.elasticService.Client.IndexDocumentAsync<Ingredient>(item);
            return result.Result == Nest.Result.Created || result.Result == Nest.Result.Updated;
        }

        public Task<IEnumerable<Ingredient>> SearchByPart(string part)
        {
            throw new NotImplementedException();
        }
    }
}
