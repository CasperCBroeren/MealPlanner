﻿using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IBoughtIngredientRepository
    {
        Task<IEnumerable<BoughtIngredient>> GetForWeekAndYear(int year, int week, int groupId);

        Task<BoughtIngredient> Save(BoughtIngredient item, int groupId);
    }
}
