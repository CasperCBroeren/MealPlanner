using System;

namespace mealplanner.Models
{
    public class WeeklyPlanning
    {
        public WeeklyPlanning(DateTime dateOfMonday, params Meal[] meals)
        {
            this.DateOfMonday = dateOfMonday;
            if (meals.Length >0) this.Monday = meals[0];
            if (meals.Length >1) this.Thursday = meals[1];
            if (meals.Length >2) this.Wednesday = meals[2];
            if (meals.Length >3) this.Thursday = meals[3];
            if (meals.Length >4) this.Friday = meals[4];
        }
        
        public Meal Monday { get;   }
        public Meal Tuesday { get; }
        public Meal Wednesday { get;  }
        public Meal Thursday { get;  }
        public Meal Friday { get;  }
        public DateTime DateOfMonday { get; }
    } 
}