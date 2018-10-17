using System;

namespace MealPlanner.Data.Models
{
    public class Tag
    {
        public int? Id { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            return ((Tag)obj).Value.Equals(Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
