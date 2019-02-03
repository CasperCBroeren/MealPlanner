using System;
using System.Collections.Generic;

namespace MealPlanner.Data.Models
{
    public class Group
    { 
        public int? GroupId { get; set; } 

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public string Secret { get; set; }
    }
}
