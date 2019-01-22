using Microsoft.AspNetCore.Identity;
using System;

namespace MealPlanner.Data.Security
{
    public class MealPlannerUser : IdentityUser<Guid>
    {
        public MealPlannerUser()
        {
        } 

        public string GroupName { get; set; } 
    }
}
