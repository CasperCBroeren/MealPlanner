using System;
using Nest;

namespace mealplanner.Models
{
    [ElasticsearchType(Name="ingredient", IdProperty="Uid")]
    public class Ingredient
    {
        public Ingredient()
        { 
            this.Uid = Guid.NewGuid();
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
 

        public Guid? Uid {get;set;}


    }
}