using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace MealPlanner.Web.Controllers
{
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        public IGroupRepository groupRepository { get; }

        public GroupController(IGroupRepository groupRepository)
        {
            this.groupRepository = groupRepository;
        }

        [Route("create/{groupName}")]
        public async Task<IActionResult> Create(string groupName)
        {
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                if (await this.groupRepository.GetByName(groupName) != null)
                {
                    return Ok("Helaas bestaat deze naam al");
                }
                var tfa = new TwoFactorAuth(groupName);
                var group = new Group()
                {
                    Name = groupName,
                    Secret = tfa.CreateSecret(160)
                };
                if (await this.groupRepository.Save(group) && group.GroupId.HasValue)
                {
                    await JoinGroup(group.GroupId.Value, groupName);

                    return new JsonResult(new
                    {
                        name = group.Name,
                        qrCode = tfa.GetQrCodeImageAsDataUri(group.Name, group.Secret)
                    });

                }
            }
            return Ok("Er is geen naam ontvangen");
        }

        [Route("getValidationToken"), HttpGet]
        [Authorize(Policy = "GroupOnly")]
        public async Task<IActionResult> GetValidationToken()
        {
            var group = (await this.groupRepository.GetById(await this.GroupId()));
            var tfa = new TwoFactorAuth(group.Name);

            return new JsonResult(new
            {
                name = group.Name,
                qrCode = tfa.GetQrCodeImageAsDataUri(group.Name, group.Secret)
            });

        }

        [Route("validate"), HttpPost]
        [Authorize(Policy = "GroupOnly")]
        public async Task<IActionResult> ValidateToken([FromBody]JObject payload)
        {
            var group = (await this.groupRepository.GetById(await this.GroupId()));
            var tfa = new TwoFactorAuth(group.Name);
            if (tfa.VerifyCode(group.Secret, payload.Property("token").Value.ToString()))
            {
                return Ok("ok");
            }
            else
            { 
                return Ok("We konden de token niet valideren, probeer het opnieuw"); 
            }
        }
         
        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinByName([FromBody] JObject payload)
        {
            if (!string.IsNullOrWhiteSpace(payload.Property("name").Value.ToString()))
            {
                var group = await this.groupRepository.GetByName(payload.Property("name").Value.ToString());
                if (group != null
                    && !string.IsNullOrWhiteSpace(payload.Property("token").Value.ToString()))
                {
                    var tfa = new TwoFactorAuth(group.Name);
                    if (tfa.VerifyCode(group.Secret, payload.Property("token").Value.ToString()))
                    {
                        if (group != null && group.GroupId.HasValue)
                        {
                            await JoinGroup(group.GroupId.Value, payload.Property("name").Value.ToString());
                            return Ok("ok");
                        }
                    }
                    else
                    {
                        return Ok("Je token is niet geledig");
                    }
                }
                else
                {
                    return Ok("Vul ook de token van je authenticator in");
                }
            }

            return Ok("Helaas kennen we deze groep niet");
           
        }
        private async Task JoinGroup(int groupdid, string name)
        {
            var claims = new List<Claim>
                    {
                        new Claim("GroupId", groupdid.ToString()),
                    };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
            this.Response.Cookies.Append(MPGGN_COOKIE_NAME, name);
        }
    }
}
