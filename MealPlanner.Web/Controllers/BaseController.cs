using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MealPlanner.Web.Controllers
{
    public class BaseController : Controller
    {
        public const string MPGG_COOKIE_NAME = "mpgg";

        public const string MPGGN_COOKIE_NAME = "mpggn";

        public string GroupGuid => this.HttpContext.Request.Cookies[MPGG_COOKIE_NAME];
         
        public string GroupName => this.HttpContext.Request.Cookies[MPGGN_COOKIE_NAME];

        public async Task<int> GroupId()
        {
            return await this.GroupRepository.GetId((this.User as ClaimsPrincipal).FindFirstValue("GroupId")); 
        }

        public IGroupRepository GroupRepository { get; }

        public BaseController(IGroupRepository groupRepository)
        {
            GroupRepository = groupRepository;
        }
    }
}
