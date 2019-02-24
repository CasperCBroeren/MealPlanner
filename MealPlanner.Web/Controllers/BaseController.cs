using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MealPlanner.Web.Controllers
{
    public class BaseController : Controller
    {
        public int? GroupId()
        {
            var result = 0;
            if (int.TryParse((this.User as ClaimsPrincipal).FindFirstValue("GroupId"), out result))
            {
                return result;
            }
            return null;
        }

        public BaseController()
        {
        }
    }
}
