using System;
using mealplanner.Models;
using Nest;

namespace mealplanner.elastic
{
    public  static class DataModelV1
    {
        public static void Setup(ElasticClient client)
        {
           // SetupIngredientsIndex(client);
            //SetupMealsIndex(client);
        }

        private static void SetupMealsIndex(ElasticClient client)
        {
            client.DeleteIndex(ElasticEndpoint.MealIndexName);
            client.CreateIndex(ElasticEndpoint.MealIndexName, 
                c=> c.Mappings(
                    ms => ms.Map<Meal>(
                        m=> m.AutoMap())
                )
            );
        }

        private static void SetupIngredientsIndex(ElasticClient client)
        {
             client.DeleteIndex(ElasticEndpoint.IngredientIndexName);
             client.CreateIndex(ElasticEndpoint.IngredientIndexName, c=>
                        c.Mappings( ms => ms.Map<Ingredient>(
                            m => m.AutoMap()
                            )
                        )
                        .Settings(s => s.Analysis(a =>  
                                        a.TokenFilters(tf => tf.EdgeNGram("autocomplete_filter", (f)=> {
                                            f.MaxGram(3);
                                            f.MinGram(1);
                                            return f;
                                        }))
                                        .Analyzers(an => 
                                            an.Custom ("autocomplete", ca => 
                                                ca
                                                .Tokenizer("standard")
                                                .Filters( "lowercase", "autocomplete_filter")))
                     )));

        
        }
    }
}