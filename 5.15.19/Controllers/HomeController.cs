using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _5._15._19.Models;
using Microsoft.Extensions.Configuration;
using Data;
using Microsoft.AspNetCore.Authorization;

namespace _5._15._19.Controllers
{
    public class HomeController : Controller
    {
        private string _conn;
        public HomeController(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("ConStr");
        }

        [Authorize]
        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    var repo = new TasksRepository(_conn);
            //    User user = repo.GetUserByEmail(User.Identity.Name);
            //    var vm = new IndexVM
            //    {
            //        Assignments = repo.GetAssignments(),
            //        UserId=user.Id
            //    };

            //    return View(vm);
            //}
            //else
            //{
            //    return Redirect("/account/login");
            //}
            var repo = new TasksRepository(_conn);
            
            return View(repo.GetIncompletedAssignments());
        }      
    }
}
