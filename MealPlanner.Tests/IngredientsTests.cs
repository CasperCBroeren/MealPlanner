using MealPlanner.Data.Elastic;
using MealPlanner.Data.Repositories.Elastic;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MealPlanner.Tests
{
    public class IngredientsTests
    {     

        [Fact]
        public async Task FindAllByName_WhenEnteredFour_ReturnTwo()
        {
            var client = new ElasticService(new Uri("http://localhost:9200"));
            DataModelV1.Setup(client.Client);
            var repo = new IngredientRepository(client);
            await repo.Save(new Data.Models.Ingredient() { Name = "Test" }); 
            await repo.Save(new Data.Models.Ingredient() { Name = "steek" }); 
            await repo.Save(new Data.Models.Ingredient() { Name = "testje" });
            await repo.Save(new Data.Models.Ingredient() { Name = "televisie" });
            
            var items = await repo.FindAllByName("e");
            items.ToList().Count.ShouldBe(2);
        }
    }
}
