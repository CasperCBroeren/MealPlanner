using MealPlanner.Data.Models;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IWeekplanningRepository
    {
        Task<Weekplanning> GetForWeekAndYear(int year, int week, int groupId);

        Task<Weekplanning> Save(Weekplanning item, int groupId );
    }
}
