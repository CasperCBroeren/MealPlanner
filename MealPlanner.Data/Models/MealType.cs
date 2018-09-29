using System;

namespace MealPlanner.Data.Models
{
    [Flags]
    public enum MealType
    {
        Unkown = 0,
        Meat = 1,
        Fish = 2, 
        Vegan = 4

    }
}