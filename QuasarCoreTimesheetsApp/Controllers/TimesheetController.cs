using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Controllers
{
    public class TimesheetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
