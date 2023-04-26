using Automata.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Automata.Controllers
{
    public class QRCodeController : BaseController
    {
        MoneraTerminalFunctions MoneraTerminal { get; set; }

        [HttpPost]
        public JsonResult ReadQR(int amount)
        {
            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            MoneraTerminal.NormalPayment(amount, "Nem tudni mi ez"); //TODO: Mi a második paraméter, honnan fogjuk tudni?


            return Json(new ResultModel() { Id = 0, Text = "" });
        }

    }
}
