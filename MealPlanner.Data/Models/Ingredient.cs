using System;
using Nest;

namespace MealPlanner.Data.Models
{
    [ElasticsearchType(Name="ingredient", IdProperty="Uid")]
    public class Ingredient
    {
        public Ingredient()
        { 
            this.Uid = Guid.NewGuid();
        } 

        public string Name { get; set; } 
          
        public Guid? Uid {get;set;}


    }
}