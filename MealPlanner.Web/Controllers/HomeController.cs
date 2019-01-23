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
        public async Task<IActionResult> Join()
        {  
            return View("join");
        }

        [Route("join"), HttpPost]
        public async Task<IActionResult> JoinByName([FromForm]string name)
        { 
            if (!string.IsNullOrEmpty(name))
            {
                var groupdGuid = await this.GroupRepository.GetByName(name);
                if (!string.IsNullOrEmpty(groupdGuid))
                {
                    SetCookieFor(groupdGuid, name);

                    var claims = new List<Claim>
                    {
                        new Claim("GroupId", groupdGuid),
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
            ViewBag.Error = "Helaas kennen we deze groep niet";
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
