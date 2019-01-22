using Dapper;
using MealPlanner.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Meal>> All(int groupId)
        {
            var query = $@"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id
                            WHERE m.groupid=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var meals = (await connection.QueryAsync(query, MealMapper(), new { groupId })).Where(x => x != null).ToList();
                await AssignTags(meals);
                return meals;
            }
        }

        private async Task AssignTags(List<Meal> meals)
        { 
            foreach (var m in meals)
            {
                IEnumerable<Tag> collection = await this.tagRepository.ForMeal(m);
                m.Tags.AddRange(collection);
            };
        }

        public async Task<bool> Delete(Meal item, int groupId)
        {
            var query = @"delete from dbo.TagsOfMeals where MealId=@id;
                          delete from dbo.IngredientsInMeals where MealId=@id;
                          delete from dbo.Meals where id=@id and groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, new { id = item.Id, groupId }) >= 1;
            }
        }

        public async Task<Meal> FindOneByName(string name, int groupId)
        {
            var query = @"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id 
                            WHERE m.name =@name and m.groupId = @groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                var items = await connection.QueryAsync(query, MealMapper(),
                new {  name, groupId });
                var item = items.Where(x => x != null).FirstOrDefault();
                if (item != null)
                {
                    await AssignTags(new List<Meal> { item });
                }
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

        public async Task<bool> Save(Meal item, int groupId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                if (item.Id.HasValue)
                {
                    var queryUpdate = $"update Meals set name=@name, description=@description, MealType=@mealtype where id=@id and groupid=@groupId";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("description", item.Description));
                    updateCommand.Parameters.Add(new SqlParameter("mealType", item.MealType));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.Id));
                    updateCommand.Parameters.Add(new SqlParameter("groupId", groupId));
                }
                else
                {
                    var queryInsert = @"insert into Meals(name, description, mealtype, groupId) values (@name, @description, @mealType, @groupId);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name,
                                                                                     description = item.Description,
                                                                                     mealType = item.MealType,
                                                                                     groupId});
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    }
                }
            }

            await SaveIngredientsOfMeal(item);
            await SaveTagsOfMeal(item, groupId);
            return true;
        }

        private async Task SaveTagsOfMeal(Meal item, int groupId)
        {
            var tags = await tagRepository.All(groupId);
            foreach (var tagNotSaved in item.Tags.Except(tags))
            {
                await this.tagRepository.Save(tagNotSaved, groupId);
            }
            var allTags = await this.tagRepository.All(groupId);
            foreach (var tag in item.Tags.Where(x => !x.Id.HasValue))
            {
                tag.Id = allTags.FirstOrDefault(x => x.Value.Equals(tag.Value, StringComparison.InvariantCultureIgnoreCase)).Id;
            }

            var deleteAllQuery = "delete TagsOfMeals where MealId=@mealid";
            var insertQuery = "insert into TagsOfMeals(MealId, TagId) values (@mealid, @tagid)";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();
                await connection.ExecuteAsync(deleteAllQuery, new { mealid = item.Id }, transaction);
                foreach (var tag in item.Tags)
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

        public async Task<IEnumerable<Meal>> FindAllByTerm(string term, int groupId)
        {
            var query = $@"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id
                            WHERE m.Name like '%'+@term+'%' and m.groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var meals = (await connection.QueryAsync(query, MealMapper(), new { groupId, term })).Where(x => x != null).ToList();
                await AssignTags(meals);
                return meals;
            }
        }

        public async Task PairMealsToDay(List<Day> days, int groupId)
        {
            if (days == null) return;
            var items = days.Select(x => x.MealId).Where(x=> x.HasValue).ToArray();
            var query = $@"SELECT * 
                            FROM [dbo].[Meals] m  
                            WHERE m.Id in @items and m.groupId = @groupId";

            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                var meals = (await connection.QueryAsync<Meal>(query, new { groupId, items })).ToList();
                foreach (var m in meals)
                {
                    days.Where(x => x.MealId == m.Id).ToList().ForEach(x => x.Meal = m);
                };
            }
        }

        public async Task<IEnumerable<Meal>> FindByIngredients(Ingredient[] ingredients, int groupId)
        {
            var query = $@"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
                                inner join (select MealId as Id  from IngredientsInMeals where IngredientId in @ingredients) selection on selection.Id =m.Id
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id
                            WHERE m.groupid=@groupId";

            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                var ingredientIds = ingredients.Select(x => x.Id).ToList();
                var meals = (await connection.QueryAsync(query, MealMapper(), new { ingredients = ingredientIds, groupId})).Where(x => x != null).ToList();
                meals = meals.Where(x => ingredientIds.All(y => x.Ingredients.Select(i => i.Id).Contains(y))).ToList();
                await AssignTags(meals);
                return meals;
            }
        }

        public async Task<IEnumerable<Meal>> FindByTagAndType(Tag[] tags, int type, int groupId)
        {
            var query = $@"SELECT distinct m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
                                inner join (select MealId as Id  from TagsOfMeals where TagId in @tags) selection on selection.Id =m.Id
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id
                                where isNull(@selectedType, MealType)=MealType and m.groupId = @groupId ";

            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                var tagIds = tags.Select(x => x.Id).ToList();
                int? selectedType = type == 0 ? new Nullable<int>() : type;
                var meals = (await connection.QueryAsync(query, MealMapper(), new { tags = tagIds,  selectedType, groupId })).Where(x => x != null).ToList();
                await AssignTags(meals);
                meals = meals.Where(x => tagIds.All(y => x.Tags.Select(i => i.Id).Contains(y))).ToList();

                return meals;
            }
        }

        public async Task<Meal> FindOneById(int id, int groupId)
        {
            var query = $@"SELECT m.Id Id, m.Name Name, m.Description Description, m.Created created, m.Mealtype mealType, 
                                 i.Id id, i.Name Name, im.Amount Amount
                            FROM [dbo].[Meals] m 
	                            left join [dbo].[IngredientsInMeals] im on im.MealId = m.Id
	                            left join [dbo].[Ingredients] i on im.IngredientId = i.Id
                           where m.Id =@id and m.groupId = @groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var meal = (await connection.QueryAsync(query, MealMapper(), new { id, groupId })).Where(x => x != null).FirstOrDefault();
                await this.tagRepository.ForMeal(meal);
                return meal;
            }
        }
    }
}
