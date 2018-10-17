using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MealPlanner.Data.Models;
using Dapper;
using System.Linq;
using System;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class MealRepository : IMealRepository
    {
        private readonly string connectionString;
        private readonly ITagRepository tagRepository;

        public ITagRepository GetTagRepository()
        {
            return tagRepository;
        }

        public MealRepository(string connectionString, ITagRepository tagRepository)
        {
            this.connectionString = connectionString;
            this.tagRepository = tagRepository;
        } 

        public async Task<IEnumerable<Meal>> All()
        {
            var query = $@"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var meals = (await connection.QueryAsync(query, MealMapper())).Where(x => x != null).ToList();
                foreach(var m in meals)
                {
                    IEnumerable<Tag> collection = await this.tagRepository.ForMeal(m);
                    m.Tags.AddRange(collection);
                };
                return meals;
            }
        }

        public async Task<bool> Delete(Meal item)
        {
            var query = @"delete from dbo.TagsOfMeals where MealId=@id;
                          delete from dbo.IngredientsInMeals where MealId=@id;
                          delete from dbo.Meals where id=@id;";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(query, new { id = item.Id }) >= 1;
            }
        }

        public async Task<Meal> FindOneByName(string name)
        {
            var query = @"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id where m.name =@name";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var items = await connection.QueryAsync(query, MealMapper(),
                new { name = name });
                var item =  items.Where(x => x != null).FirstOrDefault();
                item.Tags.AddRange(await this.tagRepository.ForMeal(item));
                return item;
            }
        }

        private static Func<Meal, IngredientAmount, Meal> MealMapper()
        {
            var mealStore = new Dictionary<int, Meal>();
            return (meal, ingredient) =>
            {
                var item = mealStore.ContainsKey(meal.Id.Value) ? mealStore[meal.Id.Value] : meal;
                if (ingredient != null)
                {
                    item.Ingredients.Add(ingredient); 
                } 

                if (!mealStore.ContainsKey(meal.Id.Value))
                {
                    mealStore.Add(meal.Id.Value, meal);
                    return item;
                }
                else
                {
                    return null;
                } 
            };
        }

        public async Task<bool> Save(Meal item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                if (item.Id.HasValue)
                {
                    var queryUpdate = $"update Meals set name=@name, description=@description, MealType=@mealtype where id=@id";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("description", item.Description));
                    updateCommand.Parameters.Add(new SqlParameter("mealType", item.MealType));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.Id)); 
                }
                else
                {
                    var queryInsert = @"insert into Meals(name, description, mealtype) values (@name, @description, @mealType);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name, description = item.Description, mealType = item.MealType });
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    } 
                }
            }

            await SaveIngredientsOfMeal(item);
           await SaveTagsOfMeal(item);
            return true;
        }

        private async Task SaveTagsOfMeal(Meal item)
        {
            var tags = await tagRepository.All();
            foreach(var tagNotSaved in item.Tags.Except(tags))
            {
               await this.tagRepository.Save(tagNotSaved);
            } 

            var deleteAllQuery = "delete TagsOfMeals where MealId=@mealid";
            var insertQuery = "insert into TagsOfMeals(MealId, TagId) values (@mealid, @tagid)";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();
                await connection.ExecuteAsync(deleteAllQuery, new { mealid = item.Id }, transaction);
                foreach(var tag in item.Tags)
                {
                    await connection.ExecuteAsync(insertQuery, new { mealid = item.Id, tagid = tag.Id }, transaction);
                }
                transaction.Commit();
                connection.Close();
            }
        }

        private async Task SaveIngredientsOfMeal(Meal item)
        {
            var deleteAllQuery = "delete IngredientsInMeals where MealId=@mealid";
            var insertQuery = "insert into IngredientsInMeals(MealId, IngredientId, Amount) values (@mealid, @ingredientid, @amount)";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();
                await connection.ExecuteAsync(deleteAllQuery, new { mealid = item.Id }, transaction);
                for (var i = 0; i < item.Ingredients.Count; i++)
                {
                    var ingredient = item.Ingredients[i]; 
                    await connection.ExecuteAsync(insertQuery, new { mealid = item.Id, ingredientid = ingredient.Id, amount = ingredient.Amount }, transaction);
                }
                transaction.Commit();
                connection.Close();
            }
        }
    }
}
