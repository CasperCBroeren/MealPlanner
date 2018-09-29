using System;
using System.Collections.Generic;
using Nest;

namespace MealPlanner.Data.Models
{
    [ElasticsearchType(Name="meal", IdProperty="Created")]
    public class Meal
    {
        public Meal()
        {
             this.Created = DateTime.Now;
             this.Ingredients = new List<Ingredient>();

             this.IngredientsAmount = new List<int>();
             this.Tags = new List<string>();
        }

    
        private string _name;

        [Text(Name = "name", Analyzer="autocomplete")]
        public string Name { get {return _name; } set {
            _name = value; 
        }} 

        [Keyword()]
        public string ExactName { get {
            return _name;
        } set {
            _name = value;
        }}

        public List<Ingredient> Ingredients { get; }

        public List<int> IngredientsAmount{get;}

        public List<string> Tags { get; }
        public MealType MealType { get; set; }

        public string Description { get; set;}

        public DateTime? Created{get;set;}

    }

}