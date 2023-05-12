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

        //private static int _tranzakcioId = 0;
        //private static int TranzakcioId
        //{
        //    get
        //    {
        //        if (++_tranzakcioId >= 1000) _tranzakcioId = 1;
        //        return _tranzakcioId;
        //    }
        //}

        [HttpPost]
        public JsonResult Fizetes(int id)
        {
            FoglalasModel model;

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            string ctid = $"PAID_{DateTime.Now:MMddHHmm}{id:D8}"; //{ TranzakcioId: D4} max 24 chars

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.NormalPayment(model.Fizetendo, ctid); //TODO: Mi a második paraméter, honnan fogjuk tudni?
            if (res == 0)
            {
                model.FizetveFl = true;
                MoneraTerminal.GetReceipt();
            }

            //return Json(new ResultModel() { Id = res, Text = MoneraTerminal.GetErrorName(res) });
            return Json(new ResultModel() { Id = 0, Text = "" });
        }

        public JsonResult PaymentPm(int amount)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 1000000000);
            string ctid = "PAID_" + randomNumber.ToString("D9"); //max 24 chars

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.NormalPayment(amount, ctid); //TODO: Mi a második paraméter, honnan fogjuk tudni?
            if (res == 0)
            {
                MoneraTerminal.GetReceipt();
            }

            return Json(new ResultModel() { Id = res, Text = MoneraTerminal.GetErrorName(res) });
            //return Json(new ResultModel() { Id = 0, Text = "" });
        }

        public JsonResult LetetZarolas(int id)
        {
            FoglalasModel model;

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            string ctid = $"DEID_{DateTime.Now:MMddHHmm}{id:D8}"; //{ TranzakcioId: D4} max 24 chars

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.DepositPayment(model.Zarolando, ctid); //TODO: Mi a második paraméter, honnan fogjuk tudni?
            if (res == 0)
            {
                model.ZarolvaFl = true;
                MoneraTerminal.GetReceipt();
            }

            //return Json(new ResultModel() { Id = res, Text = MoneraTerminal.GetErrorName(res) });
            return Json(new ResultModel() { Id = 0, Text = "" });
        }

        public JsonResult DepositPm(int amount)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 1000000000);
            string ctid = "DEID_" + randomNumber.ToString("D9"); //max 24 chars

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.DepositPayment(amount, ctid); //TODO: Mi a második paraméter, honnan fogjuk tudni?
            if (res == 0)
            {
                MoneraTerminal.GetReceipt();
            }

            return Json(new ResultModel() { Id = res, Text = MoneraTerminal.GetErrorName(res) });
            //return Json(new ResultModel() { Id = 0, Text = "" });
        }

        [HttpPost]
        public JsonResult FizetesRendben(int id)
        {
            FoglalasModel model;

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            return Json(new ResultModel() { Id = (!model.FizetveFl).GetHashCode(), Text = "" });
        }

        [HttpPost]
        public JsonResult LetetZarolasRendben(int id)
        {
            FoglalasModel model;

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out model))
                throw new Exception("No such reservation");

            return Json(new ResultModel() { Id = (!model.ZarolvaFl).GetHashCode(), Text = "" });
        }

        public JsonResult Cancel()
        {
            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.CancelPayment();

            return Json(new ResultModel() { Id = res, Text = MoneraTerminal.GetErrorName(res) });
        }
    }
}
