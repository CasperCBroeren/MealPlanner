using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IMealRepository
    {
        Task<IEnumerable<Meal>> All();

        Task<Meal> FindOneByName(string name);

        Task<bool> Save(Meal item);

        Task<bool> Delete(Meal item);

        Task<IEnumerable<Meal>> FindAllByTerm(string term);

        Task PairMealsToDay(List<Day> days);

        Task<IEnumerable<Meal>> FindByIngredients(Ingredient[] ingredients);

        Task<IEnumerable<Meal>> FindByTagAndType(Tag[] tags, int type);

        Task<Meal> FindOneById(int id);
    }
}
