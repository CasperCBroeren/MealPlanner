using Dapper;
using MealPlanner.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class BoughtIngredientRepository : IBoughtIngredientRepository
    {
        private readonly string connectionString;

        public BoughtIngredientRepository(IConfiguration config)
        {
            this.connectionString = config["dbConnectionString"];
        }

        public async Task<IEnumerable<BoughtIngredient>> GetForWeekAndYear(int year, int week, int groupId)
        {
            var query = $@"SELECT   i.Id as IngredientId,bi.id,  bi.Bought, PivotDays.id as WeekplanId, im.Amount as Amount, PivotDays.Day,
                                    i.Id id, i.Name Name
                             FROM  
								 (select id, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday from WeekPlans wp
								 
								 where [Week] = @week and [Year]=@year and [groupId]=@groupId) w
								 UNPIVOT
								 (
									Mealid for Day in (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)
								 ) as PivotDays
								left join [dbo].Meals m on m.Id = PivotDays.Mealid
								left join [dbo].[IngredientsInMeals] im on im.MealId=m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id 
	                            left join [dbo].[BoughtIngredients] bi   on bi.Ingredient=i.id
																		and bi.Day = PivotDays.Day	
																		and bi.WeekPlanId=PivotDays.Id
                                where i.Id is not null
                              order by Name";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var meals = (await connection.QueryAsync(query, BoughtIngredientMapper(), new { year, week, groupId})).ToList();
                return meals;
            }
        }

        private static Func<BoughtIngredient, Ingredient, BoughtIngredient> BoughtIngredientMapper()
        {
            return (boughtIngredient, ingredient) =>
            {
                boughtIngredient.Ingredient = ingredient;
                return boughtIngredient;
            };
        }

        public async Task<BoughtIngredient> Save(BoughtIngredient item, int groupId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"
                              update BoughtIngredients set Bought = @date where weekplanid=@weekplan and Day=@day and ingredient=@ingredient;
                              IF @@ROWCOUNT=0
                              Begin
                                insert into BoughtIngredients(Bought, WeekPlanId, Day, Ingredient, GroupId) values(@date, @weekplan, @day, @ingredient, @groupId);
                              
                                select @@identity
                              end";

                var id = await connection.QueryFirstOrDefaultAsync<int>(query, new { date = item.Bought,
                                                                                     weekplan = item.WeekPlanId,
                                                                                     day = item.Day,
                                                                                     ingredient = item.Ingredient.Id,
                                                                                     groupId});

                item.Id = id > 0 ? id : item.Id;
                return item;
            }
        }
    }
}
