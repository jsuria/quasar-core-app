using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuasarCoreTimesheetsApp.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        } 
    }
}
