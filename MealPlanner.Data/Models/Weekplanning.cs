using System.Collections.Generic;
using System.Data;

namespace MealPlanner.Data.Models
{
    public class Weekplanning
    {
        public Weekplanning()
        {
           
        }

        public int? Id { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }

        public List<Day> Days { get; set; } 
        public int? Friday { get => Days[5].MealId; set => Days[5].MealId = value; }
        public int? Saturday { get => Days[6].MealId; set => Days[6].MealId = value; }
        public int? Sunday { get => Days[0].MealId; set => Days[0].MealId = value; }
        public int? Monday { get => Days[1].MealId; set => Days[1].MealId = value; }
        public int? Tuesday { get => Days[2].MealId; set => Days[2].MealId = value; }
        public int? Wednesday { get => Days[3].MealId; set => Days[3].MealId = value; }
        public int? Thursday { get => Days[4].MealId; set => Days[4].MealId = value; }
    }
}
