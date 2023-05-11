using Automata.Functions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Automata.Controllers
{
    public class PrinterController : BaseController
    {
        IPrinterFunctions PrinterFunctions { get; set; }

        public PrinterController(IPrinterFunctions printerFunctions)
        {
            PrinterFunctions = printerFunctions;
        }

        [HttpPost]
        public JsonResult PrintReceiptHun(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber)
        {
            var result = PrinterFunctions.PrintReceiptHun(agreementNumber, plateNumber, endOfRental, money, preAuthorizationNumber);
            return Json(result);
        }

        [HttpPost]
        public JsonResult PrintReceiptEng(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber)
        {
            var result = PrinterFunctions.PrintReceiptEng(agreementNumber, plateNumber, endOfRental, money, preAuthorizationNumber);
            return Json(result);
        }

    }
}
