using System;
using System.Collections.Generic;

namespace MealPlanner.Data.Models
{
    public class Group
    {
        public List<string> PhoneNumbers { get; set; }
        public int? GroupId { get; set; } 

        public string Name { get; set; }

        public DateTime Created { get; set; }
    }
}
