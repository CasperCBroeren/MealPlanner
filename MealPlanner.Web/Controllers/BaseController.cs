using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{
    public class BaseController : Controller
    { 
        public const string MPGGN_COOKIE_NAME = "mpggn";
         
        public async Task<int> GroupId()
        {
            return int.Parse((this.User as ClaimsPrincipal).FindFirstValue("GroupId")); 
        }

        public IGroupRepository GroupRepository { get; }

        public BaseController(IGroupRepository groupRepository)
        {
            GroupRepository = groupRepository;
        }
    }
}
