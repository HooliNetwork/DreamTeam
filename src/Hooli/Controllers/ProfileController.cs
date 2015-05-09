using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Hooli.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Hooli.Controllers
{
    public class ProfileController : Controller
    {
        public HooliContext DbContext { get; set; }
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
