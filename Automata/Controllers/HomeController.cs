using FunctionsCore;
using FunctionsCore.Attributes;
using FunctionsCore.Commons;
using FunctionsCore.Commons.EntitiesJson;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Enums;
using FunctionsCore.Models;
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


        public JsonResult GetData()
        {
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();
            var nyelv = Request.Headers["Accept-Language"];
            var model = new FoglalasModel() { Id = 12, KezdDatum = DateTime.Now, Fizetendo = 10, Zarolando = 12, Nev = "Gábor", Tipus = "admin", Nyelv = Nyelvek.hu };
            BookingFunctions.UjFoglalas(model);

            var foglalas = BookingFunctions.FindFoglalasById(12);
            if (foglalas != null)
            {
                switch (nyelv)
                {
                    case "en":
                        foglalas.Nyelv = Nyelvek.en;
                        break;
                    case "hu":
                        foglalas.Nyelv = Nyelvek.hu;
                        break;
                }
                BookingFunctions.UjFoglalas(foglalas);

            }

            return Json(model);
        }

        public JsonResult Driver()
        {
            DriverFunctions.Main(null);
            return EmptyJson();
        }

       
    }
}
