using System;

namespace MealPlanner.Data.Models
{
    public class BoughtIngredient
    {
        public int Id { get; set; }

        public int IngredientId { get; set; }

        public DateTime? Bought { get; set; }

        public int WeekPlanId { get; set; }


        public string Amount { get; set; }

        public string Day { get; set; }

        public Ingredient Ingredient { get; set; }
        
    }
}
