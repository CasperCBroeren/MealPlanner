using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> All(int groupId);

        Task<IEnumerable<Tag>> ForMeal(Meal meal);
        
        Task<bool> Delete(Tag item, int groupId);

        Task<bool> Save(Tag item, int groupId);
        Task<IEnumerable<Tag>> FindStartingWith(string startWith, int groupId);
        Task<Tag> Find(string tag, int groupId);
    }
}
