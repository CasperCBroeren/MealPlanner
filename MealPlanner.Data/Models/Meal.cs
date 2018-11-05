using System;
using System.Collections.Generic; 

namespace MealPlanner.Data.Models
{ 
    public class Meal
    {
        public Meal()
        {
             this.Created = DateTime.Now;
             this.Ingredients = new List<IngredientAmount>();
             
             this.Tags = new List<Tag>();
        }

        public int? Id { get; set; }

        public string Name { get; set; }

        public List<IngredientAmount> Ingredients { get; }
         
        public List<Tag> Tags { get; }
        public MealType MealType { get; set; }

        public string Description { get; set;}

        public DateTime? Created{get;set;}

    }

}