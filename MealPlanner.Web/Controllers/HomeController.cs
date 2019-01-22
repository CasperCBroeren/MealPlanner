using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IGroupRepository groupRepository) : base(groupRepository)
        {
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
                if (await this.GroupRepository.ExistsByName(groupName))
                {
                    ViewBag.Error = "Helaas bestaat deze naam al";
                    return View("new");
                }
                var guid = await this.GroupRepository.AddNew(groupName);
                SetCookieFor(guid, groupName);
                return new RedirectResult("/");
            }
            return View("new");
        }

        [Route("join"), HttpGet]
        public async Task<IActionResult> Join([FromQuery]string g)
        { 

            if (!string.IsNullOrEmpty(g))
            {
                var name = await this.GroupRepository.GetName(g);
                if (!string.IsNullOrEmpty(name))
                {
                    SetCookieFor(g, name);

                    var claims = new List<Claim>
                    {
                        new Claim("GroupId", g),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);
                    return new RedirectResult("/");
                }
            }
            return View("join");
        }


        private void SetCookieFor(string guid, string name)
        {
            this.Response.Cookies.Append(MPGG_COOKIE_NAME, guid);
            this.Response.Cookies.Append(MPGGN_COOKIE_NAME, name);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
