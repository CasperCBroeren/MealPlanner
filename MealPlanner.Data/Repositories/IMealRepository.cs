using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IMealRepository
    {
        Task<IEnumerable<Meal>> All(int groupId);

        Task<Meal> FindOneByName(string name, int groupId);

        Task<bool> Save(Meal item, int groupId);

        Task<bool> Delete(Meal item, int groupId);

        Task<IEnumerable<Meal>> FindAllByTerm(string term, int groupId);

        Task PairMealsToDay(List<Day> days, int groupId);

        Task<IEnumerable<Meal>> FindByIngredients(Ingredient[] ingredients, int groupId);

        Task<IEnumerable<Meal>> FindByTagAndType(Tag[] tags, int type, int groupId);

        Task<Meal> FindOneById(int id, int groupId);
    }
}
