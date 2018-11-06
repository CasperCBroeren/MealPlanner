using MealPlanner.Data.Models;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IWeekplanningRepository
    {
        Task<Weekplanning> GetForWeekAndYear(int year, int week);

        Task<Weekplanning> Save(Weekplanning item);
    }
}
