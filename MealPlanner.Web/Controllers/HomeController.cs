using MealPlanner.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IGroupRepository groupRepository { get; }

        public HomeController(IGroupRepository groupRepository) 
        {
            this.groupRepository = groupRepository;
        }

        public IActionResult Index()
        {
            return View();
        } 

        public IActionResult Error()
        {
            return View();
        }
    }
}
