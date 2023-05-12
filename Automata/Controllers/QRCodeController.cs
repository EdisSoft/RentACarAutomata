using Automata.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Automata.Controllers
{
    public class QrCodeController : BaseController
    {
        [HttpPost]
        public JsonResult Start()
        {
            return Json(new ResultModel() { Id = 0, Text = "" });
        }
    }
}
