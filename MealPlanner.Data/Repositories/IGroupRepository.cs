using MealPlanner.Data.Models;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IGroupRepository
    {
        Task<bool> Save(Group group);  
         
        Task<Group> GetByName(string name);
        Task<Group> GetById(int groupId);
    }
}
