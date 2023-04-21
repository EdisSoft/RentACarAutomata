using Automata.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Automata.Controllers
{
    public class PosController : BaseController
    {
        MoneraTerminalFunctions MoneraTerminal { get; set; }

        [HttpPost]
        public JsonResult Payment(int amount)
        {
            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            MoneraTerminal.NormalPayment(amount, "Nem tudni mi ez"); //TODO: Mi a második paraméter, honnan fogjuk tudni?


            return Json(new ResultModel() { Id = 0, Text = "" });
        }
    }
}
