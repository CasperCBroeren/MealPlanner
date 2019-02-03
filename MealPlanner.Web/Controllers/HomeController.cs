using MealPlanner.Data.Models;
using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TwoFactorAuthNet;

namespace MealPlanner.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IGroupRepository groupRepository { get; }

        public HomeController(IGroupRepository groupRepository) 
        {
            this.groupRepository = groupRepository;
        }
        [Authorize(Policy = "GroupOnly")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("new")]
        public IActionResult New()
        {
            return View();
        }

        [Route("startnew"), HttpPost]
        public async Task<IActionResult> StartNew(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                if (await this.groupRepository.GetByName(groupName) != null)
                {
                    ViewBag.Error = "Helaas bestaat deze naam al";
                    return View("new");
                }
                var group = new Group()
                {
                    Name = groupName
                };
                if (await this.groupRepository.Save(group) && group.GroupId.HasValue)
                {
                    await JoinGroup(group.GroupId.Value, groupName);

                    return new RedirectResult("/validateToken");
                }
            }
            return View("new");
        } 

        [Route("validateToken"), HttpGet]
        [Authorize(Policy = "GroupOnly")]
        public async Task<IActionResult> StartValidateToken()
        {
            var group = (await this.groupRepository.GetById(await this.GroupId()));
            var tfa = new TwoFactorAuth(group.Name);
            if (string.IsNullOrWhiteSpace(group.Secret))
            {
                group.Secret = tfa.CreateSecret(160);
                await this.groupRepository.Save(group);
            }
            
            var qrToken = tfa.GetQrCodeImageAsDataUri("Maaltijdplanner", group.Secret);

            return View("validateToken", qrToken);
        }

        [Route("validateToken"), HttpPost]
        [Authorize(Policy = "GroupOnly")]
        public async Task<IActionResult> ValidateToken([FromForm] string token)
        {
            var group = (await this.groupRepository.GetById(await this.GroupId()));
            var tfa = new TwoFactorAuth(group.Name);
            if (tfa.VerifyCode(group.Secret, token))
            {
                return new RedirectResult("/");
            }
            else
            {
                var qrToken = tfa.GetQrCodeImageAsDataUri("Maaltijdplanner", group.Secret);
                ViewBag.Error = "We konden de token niet valideren, probeer het opnieuw";
                return View("validateToken", qrToken);

            }
        }

        [Route("join"), HttpGet]
        public async Task<IActionResult> Join()
        {
            return View("join");
        }

        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinByName([FromForm]string name, [FromForm] string token)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var group = await this.groupRepository.GetByName(name);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var tfa = new TwoFactorAuth(group.Name);
                    if (tfa.VerifyCode(group.Secret, token))
                    {
                        if (group != null && group.GroupId.HasValue)
                        {
                            await JoinGroup(group.GroupId.Value, name);
                            return new RedirectResult("/");
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Je token is niet geledig";
                    }
                }
                else
                {
                    ViewBag.Error = "Vul ook de token van je authenticator in";
                }
            }
            else
            {
                ViewBag.Error = "Helaas kennen we deze groep niet";
            }
            return View("join");
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
