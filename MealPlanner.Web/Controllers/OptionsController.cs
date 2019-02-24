
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using TwoFactorAuthNet;
using Newtonsoft.Json.Linq;

namespace MealPlanner.Web.Controllers
{
    [Route("api/[controller]"),
        Authorize(Policy = "GroupOnly")] 
    public class OptionsController : BaseController
    {
        private IGroupRepository groupRepository;

        public OptionsController(IGroupRepository groupRepository, IIngredientRepository ingredientRepository)
        {
            this.groupRepository = groupRepository;
        }

        [HttpGet("")]
        public async Task<ActionResult> All()
        {
            var group = (await this.groupRepository.GetById(this.GroupId().Value));
            var tfa = new TwoFactorAuth(group.Name);
            
            if (string.IsNullOrWhiteSpace(group.Secret))
            {    
                group.Secret = tfa.CreateSecret(160);
                await this.groupRepository.Save(group);
            }
            return base.Ok(new
            {  
                QrToken = tfa.GetQrCodeImageAsDataUri("Maaltijdplanner", group.Secret)
            }); 
        }
                
        [HttpGet("[action]")]
        public async Task<ActionResult> Validate([FromBody] JObject validation)
        {
            var group = (await this.groupRepository.GetById(this.GroupId().Value));
            var tfa = new TwoFactorAuth(group.Name); 
            if (tfa.VerifyCode(group.Secret, validation.Property("token").Value.ToString()))
            {
                return Ok("Correct, klaar om te gebruiken");
            }
            return Ok("Validatie is incorrect");
        } 
    }

}
