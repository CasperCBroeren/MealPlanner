using MealPlanner.Data.Models;
using Nest;
using System; 

namespace MealPlanner.Data.Elastic
{
    public class ElasticService
    {
        public const string MealIndexName = "meals";
        public const string IngredientIndexName = "ingredients";
        public ElasticClient Client { get; }
        public ElasticService(Uri elasticEndpoint)
        {
            var setting = new ConnectionSettings(elasticEndpoint);
            setting.DefaultMappingFor<Meal>(m => m.IndexName(MealIndexName));
            setting.DefaultMappingFor<Ingredient>(m => m.IndexName(IngredientIndexName));

            Client = new ElasticClient(setting);
            DataModelV1.Setup(Client);
        }
    }
}