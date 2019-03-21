using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace MealPlanner.Web.Controllers
{
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private readonly IGroupRepository groupRepository;
        private readonly IConfiguration configuration;

        public GroupController(IGroupRepository groupRepository,
                               IConfiguration config)
        {
            this.groupRepository = groupRepository;
            configuration = config;
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
                    var jwt = JoinGroupJwtBased(group);
                    return new JsonResult(new
                    {
                        name = group.Name,
                        qrCode = tfa.GetQrCodeImageAsDataUri(group.Name, group.Secret),
                        token = jwt
                    });

                }
            }
            return Ok("Er is geen naam ontvangen");
        }

        [Route("logout")]
        public IActionResult Logout()
        {

            return Ok();
        }

        private string JoinGroupJwtBased(Group group)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["jwtSecret"]);
            var tokenDiscriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim("GroupId", group.GroupId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = "MealPlanner",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDiscriptor);
            return tokenHandler.WriteToken(token);
        }

        [Route("getValidationToken"), HttpGet] 
        public async Task<IActionResult> GetValidationToken()
        {
            var groupId = this.GroupId();
            if (groupId.HasValue)
            {
                var group = (await this.groupRepository.GetById(groupId.Value));
                var tfa = new TwoFactorAuth(group.Name);

                return new JsonResult(new
                {
                    name = group.Name,
                    qrCode = tfa.GetQrCodeImageAsDataUri(group.Name, group.Secret)
                });
            }

            return new JsonResult("nope");
        }

        [Route("validate"), HttpPost]
        [Authorize(Policy = "GroupOnly")]
        public async Task<IActionResult> ValidateToken([FromBody]JObject payload)
        {
            var group = (await this.groupRepository.GetById(this.GroupId().Value));
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
                            var jwt = JoinGroupJwtBased(group);
                            return new JsonResult(new
                            {
                                name = group.Name, 
                                token = jwt
                            });
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
    }
}
