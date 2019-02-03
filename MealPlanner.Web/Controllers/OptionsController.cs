
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using TwoFactorAuthNet;

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
            var group = (await this.groupRepository.GetById(await this.GroupId()));
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
                
        [HttpGet("[action]/{validation}")]
        public async Task<ActionResult> Validate(string validation)
        {
            var group = (await this.groupRepository.GetById(await this.GroupId()));
            var tfa = new TwoFactorAuth(group.Name); 
            if (tfa.VerifyCode(group.Secret, validation))
            {
                return Ok("CORRECT");
            }
            return Ok("INCORRECT");
        }

         
    }

}
