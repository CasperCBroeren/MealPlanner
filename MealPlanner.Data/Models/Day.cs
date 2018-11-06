namespace MealPlanner.Data.Models
{
    public class Day
    {     
        public Day()
        {

        }

        public Day(string dayname, int? mealId)
        {
            this.DayName = dayname;
            this.MealId = mealId;
        }

        
        public string DayName { get; set; }
        public int? MealId { get; set; }

        public Meal Meal { get; set; }
    }
}