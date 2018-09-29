﻿using MealPlanner.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories
{
    public interface IMealRepository
    {
        Task<IList<Meal>> All();

        Task<Meal> FindOneByName(string name);

        Task<bool> Save(Meal item);

        Task<bool> Delete(Meal item);
    }
}
