namespace MealPlanner.Data.Models
{
    public class Day
    {     
        public Day(string dayname)
        {
            this.DayName = dayname;
        }

        
        public string DayName { get; set; }
        public int? MealId { get; set; }

        public Meal Meal { get; set; }
    }
}