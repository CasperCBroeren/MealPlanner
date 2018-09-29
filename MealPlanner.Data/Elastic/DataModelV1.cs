using System;
using MealPlanner.Data.Models;
using Nest;

namespace MealPlanner.Data.Elastic
{
    public  static class DataModelV1
    {
        public static void Setup(ElasticClient client)
        {
           SetupIngredientsIndex(client);
           SetupMealsIndex(client);
        }

        private static void SetupMealsIndex(ElasticClient client)
        {
            client.DeleteIndex(ElasticService.MealIndexName);
            client.CreateIndex(ElasticService.MealIndexName, 
                c=> c.Mappings(
                    ms => ms.Map<Meal>(
                        m=> m.AutoMap())
                )
            );
        }

        private static void SetupIngredientsIndex(ElasticClient client)
        {
             client.DeleteIndex(ElasticService.IngredientIndexName);
             client.CreateIndex(ElasticService.IngredientIndexName, c=>
                        c.Mappings( ms => ms.Map<Ingredient>(
                            m => m.Properties(ps => ps
                                                      .Keyword( s=> s.Name( e=> e.Name).Fields( ff=> ff.Completion(cc => cc.Name("name_suggester"))))
                                                      .Keyword( s=> s.Name( e=> e.Uid))
                            )
                        ))
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