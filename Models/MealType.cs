using System;

namespace mealplanner {
    [Flags]
    public enum MealType
    {
        Unkown = 0,
        Meat = 1,
        Fish = 2, 
        Vegan = 4

    }
}