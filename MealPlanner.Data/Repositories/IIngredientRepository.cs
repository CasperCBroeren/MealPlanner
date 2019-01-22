using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> All(int groupId);

        Task<Ingredient> FindSingleByName(int groupId, string name);

        Task<IEnumerable<Ingredient>> FindAllByName(int groupId, string name);
 
        Task<bool> Delete(int groupId, Ingredient item);

        Task<bool> Save(int groupId, Ingredient item);

        Task<IEnumerable<Ingredient>> SearchByPart(int groupId, string part);
    }
}
