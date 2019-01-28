using MealPlanner.Data.Models;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IGroupRepository
    {
        Task<bool> Save(Group name);  
         
        Task<Group> GetByName(string name);
    }
}
