using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealPlanner.Data.Elastic;
using MealPlanner.Data.Models;

namespace MealPlanner.Data.Repositories.Elastic
{
    public class MealRepository : IMealRepository
    {
        private readonly ElasticService elasticService;

        public MealRepository(ElasticService elasticService)
        {
            this.elasticService = elasticService;
        }

        public async Task<IEnumerable<Meal>> All()
        {
            var items = await this.elasticService.Client.SearchAsync<Meal>(s => s.MatchAll().Index(ElasticService.MealIndexName));
            return items.Documents.ToList();
        }

        public async Task<bool> Delete(Meal item)
        {
            var result = await this.elasticService.Client.DeleteByQueryAsync<Meal>(q => q.Query(rq => rq.Term(t => t.Created, item.Created)));
            return result.Deleted > 0;
        }

        public Task<IEnumerable<Meal>> FindAllByTerm(string term)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Meal>> FindByIngredients(Ingredient[] ingredients)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Meal>> FindByTagAndType(Tag[] tags, int type)
        {
            throw new System.NotImplementedException();
        }

        public Task<Meal> FindOneById(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Meal> FindOneByName(string name)
        { 
            var item = await this.elasticService.Client.SearchAsync<Meal>(s => s.Query(q => q.Term(t => t.Name, name)).Index(ElasticService.MealIndexName));
            return item.Documents.Count > 0 ? item.Documents.First() : null;
        }

        public Task PairMealsToDay(List<Day> days)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Save(Meal item)
        {
            var result = await this.elasticService.Client.IndexDocumentAsync(item);
            return result.Result == Nest.Result.Created || result.Result == Nest.Result.Updated;
        }
    }
}
