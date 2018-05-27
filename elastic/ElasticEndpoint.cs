using System;
using mealplanner.Models;
using Nest;
using Umi.Core;


public static class ElasticEndpoint
{
    public const string MealIndexName = "meals";
    public const string IngredientIndexName = "ingredients";
    private  static ElasticClient _client;
        public static ElasticClient Instance
        {
            get
            {
                if (_client == null)
                {
                    var elasticEndpoint = new Uri("http://localhost:9200/").RegisterAsEndpoint(config => {
                        config.Category = "ElasticSearch"; 
                    });
                    var setting = new ConnectionSettings(elasticEndpoint);  
                    setting.DefaultMappingFor<Meal>(m=> m.IndexName(MealIndexName));
                    setting.DefaultMappingFor<Ingredient>(m=> m.IndexName(IngredientIndexName));

                    _client =  new ElasticClient(setting);
                    
                        
                }
                return _client;
            }
        }
}