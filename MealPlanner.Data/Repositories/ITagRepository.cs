using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> All();

        Task<IEnumerable<Tag>> ForMeal(Meal meal);
        
        Task<bool> Delete(Tag item);

        Task<bool> Save(Tag item);
        Task<IEnumerable<Tag>> FindStartingWith(string startWith);
    }
}
