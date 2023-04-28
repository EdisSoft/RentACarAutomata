using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Automata.Controllers
{
    public class PosController : BaseController
    {
        MoneraTerminalFunctions MoneraTerminal { get; set; }

        [HttpPost]
        public JsonResult Payment(int id)
        {
            FoglalasModel model;
            if (!DelyveryFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            MoneraTerminal.NormalPayment(model.Fizetendo, "Nem tudni mi ez"); //TODO: Mi a második paraméter, honnan fogjuk tudni?

            return Json(new ResultModel() { Id = 0, Text = "" });
        }

        public JsonResult LetetZarolas(int id)
        {
            FoglalasModel model;
            if (!DelyveryFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            MoneraTerminal.DepositPayment(model.Zarolando, "Nem tudni mi ez"); //TODO: Mi a második paraméter, honnan fogjuk tudni?

            return Json(new ResultModel() { Id = 0, Text = "" });
        }
    }
}
