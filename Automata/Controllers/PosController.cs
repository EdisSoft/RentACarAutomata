using Automata.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;

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

        public JsonResult LetetZarolas(int id)
        {
            int amount = 0;

            //TODO: Le kell kérni a CRM-től a foglalási és finanszírozási adatokat, ezeket az első hívás tartalmazza, vagy ha eltettük akkor visszakeressük

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            MoneraTerminal.DepositPayment(amount, "Nem tudni mi ez"); //TODO: Mi a második paraméter, honnan fogjuk tudni?


            return Json(new ResultModel() { Id = 0, Text = "" });
        }
    }
}
