using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IGroupRepository
    {
        Task<string> AddNew(string name);
 
        Task<bool> ExistsByName(string groupName);

        Task<string> GetName(string groupGuid);
        Task<int> GetId(string groupGuid);
    }
}
