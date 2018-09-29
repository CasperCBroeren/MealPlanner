using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IIngredientRepository
    {
        Task<IList<Ingredient>> All();

        Task<Ingredient> FindSingleByName(string name);

        Task<IList<Ingredient>> FindAllByName(string name);
 
        Task<bool> Delete(Ingredient item);

        Task<bool> Save(Ingredient item);
    }
}
