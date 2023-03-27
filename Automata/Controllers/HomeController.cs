using FunctionsCore;
using FunctionsCore.Attributes;
using FunctionsCore.Commons;
using FunctionsCore.Commons.EntitiesJson;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Enums;
using FunctionsCore.Utilities.Extension.StringExtension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Automata.Controllers
{
    [TrackTimeFilter]
    public class HomeController : BaseController
    {

        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            _env = env;
        }

        public JsonResult GetData() {
            var list = new BaseDbContext().Cimkek.Take(2).Select(x => new { x.Id, x.Nev }).ToList();
            return Json(list);
        }

        public JsonResult Driver()
        {
            DriverFunctions.Main(null);
            return EmptyJson();
        }

       
    }
}
