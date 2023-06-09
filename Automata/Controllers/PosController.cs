﻿using Automata.Functions;
using FunctionsCore;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Enums;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Automata.Controllers
{
    public class PosController : BaseController
    {
        private MoneraTerminalFunctions MoneraTerminal { get; set; }
        private IPrinterFunctions PrinterFunctions { get; set; }
        private IBookingFunctions BookingFunctionsInst { get; set; }

        Thread posThread;

        public PosController(IPrinterFunctions printerFunctions, IBookingFunctions bookingFunctions)
        {
            PrinterFunctions = printerFunctions;
            BookingFunctionsInst = bookingFunctions;
        }

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
            /*{
                // For testing printer functions
                MoneraReceiptModel moneraReceipt = new MoneraReceiptModel();
                string sReceipt = "TID=02439406|ATH=227187  |RETNUM=001|RETTXT=ELFOGADVA|AMT=11,00|DATE=2023.04.28 23:03:43|" +
                    "CNB=478738XXXXXX1811|REFNO=17|ACQ=OTP BANK|CTYP=Visa Card|LOC=VECSE'S FO\" UTCA 195|MERCN=GAME RENTACAR KFT.|" +
                    "OWN=GAME RENTACAR|AID=A0000000031010|TC=B3112CA3096DF044|TRID=PAID_19338";
                moneraReceipt.Parse(sReceipt);
                Log.Info("Printing test payment receipt");
                PrinterFunctions.PrintOtpResult(moneraReceipt);
                return Json(new ResultModel() { Id = 0, Text = "Test printing" });
            }*/
            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            JsonResult res = FizetesFolyamat(model);

            return res;
        }

        public JsonResult FizetesFolyamat(FoglalasModel model)
        {
            model.FizetesMegszakadtFl = false;
            model = BookingFunctions.UjFoglalasVagyModositas(model);

            string ctid = $"PAID_{DateTime.Now:MMddHHmm}{model.Id:D8}"; //{ TranzakcioId: D4} max 24 chars

#if DEBUG
            model.FizetveFl = true;
            model = BookingFunctions.UjFoglalasVagyModositas(model);
            BookingFunctions.UpdateUtolsoVarazsloLepes(model.Id, 10); // 9+1
            BookingFunctionsInst.UjCsomag(new DeliveryModel()
            {
                OrderId = model.Id,
                ValueInt = 1,
                ValueStr = "Split szöveg",
                ValueNyelv = model.Nyelv,
                Type = DeliveryTypes.Payment
            });
            return Json(new ResultModel() { Id = 0, Text = "" });
#endif

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            if (model.Nyelv == Nyelvek.hu)
            {
                MoneraTerminal.SetLanguageHun();
            }
            else
            {
                MoneraTerminal.SetLanguageEng();
            }

            int res = MoneraTerminal.NormalPayment(model.Fizetendo, ctid); //TODO: Mi a második paraméter, honnan fogjuk tudni?
            //int res = 0;
            if (res == 0)
            {
                model.FizetveFl = true;
                model = BookingFunctions.UjFoglalasVagyModositas(model);
                BookingFunctions.UpdateUtolsoVarazsloLepes(model.Id, 10); // 9+1

                MoneraReceiptModel moneraReceipt = new MoneraReceiptModel();

                string printLn = MoneraTerminal.GetReceipt();
                moneraReceipt.Parse(printLn);

                //string sReceipt = "TID=02439406|ATH=227187  |RETNUM=001|RETTXT=ELFOGADVA|AMT=11,00|DATE=2023.04.28 23:03:43|" + 
                //    "CNB=478738XXXXXX1811|REFNO=17|ACQ=OTP BANK|CTYP=Visa Card|LOC=VECSE'S FO\" UTCA 195|MERCN=GAME RENTACAR KFT.|" + 
                //    "OWN=GAME RENTACAR|AID=A0000000031010|TC=B3112CA3096DF044|TRID=PAID_19338";
                //moneraReceipt.Parse(sReceipt);

                Log.Info("Printing payment receipt");
                PrinterFunctions.PrintOtpResult(moneraReceipt);

                int authCode;
                int.TryParse(Regex.Replace(moneraReceipt.AuthCode, @"[^\d]", ""), out authCode); // Csak számokat tartalmazzon

                BookingFunctionsInst.UjCsomag(new DeliveryModel()
                {
                    OrderId = model.Id,
                    ValueInt = authCode,
                    ValueStr = printLn,
                    ValueNyelv = model.Nyelv,
                    Type = DeliveryTypes.Payment
                });
            }
            else
            {
                model.FizetesMegszakadtFl = true;
                BookingFunctions.UjFoglalasVagyModositas(model);
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
                MoneraReceiptModel moneraReceipt = new MoneraReceiptModel();
                moneraReceipt.Parse(MoneraTerminal.GetReceipt());

                PrinterFunctions.PrintReceiptHun("aggreeNum", "plateNum", System.DateTime.Today, ((int)Double.Parse(moneraReceipt.Amount)), moneraReceipt.AuthCode);
            }

            return Json(new ResultModel() { Id = res, Text = MoneraTerminalFunctions.GetErrorName(res) });
            //return Json(new ResultModel() { Id = 0, Text = "" });
        }

        public JsonResult LetetZarolas(int id)
        {
            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            JsonResult res = LetetZarolasFolyamat(model);

            return res;
        }

        public JsonResult LetetZarolasFolyamat(FoglalasModel model)
        {
            model.ZarolasMegszakadtFl = false;
            model = BookingFunctions.UjFoglalasVagyModositas(model);

            string ctid = $"DEID_{DateTime.Now:MMddHHmm}{model.Id:D8}"; //{ TranzakcioId: D4} max 24 chars

#if DEBUG
            model.ZarolvaFl = true;
            model = BookingFunctions.UjFoglalasVagyModositas(model);
            BookingFunctions.UpdateUtolsoVarazsloLepes(model.Id, 9); // 8+1
            return Json(new ResultModel() { Id = 0, Text = "" });
#endif

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            if (model.Nyelv == Nyelvek.hu)
            {
                MoneraTerminal.SetLanguageHun();
            }
            else
            {
                MoneraTerminal.SetLanguageEng();
            }

            int res = MoneraTerminal.DepositPayment(model.Zarolando, ctid);
            //int res = 0;
            if (res == 0)
            {
                model.ZarolvaFl = true;
                model = BookingFunctions.UjFoglalasVagyModositas(model);
                BookingFunctions.UpdateUtolsoVarazsloLepes(model.Id, 9); // 8+1

                MoneraReceiptModel moneraReceipt = new MoneraReceiptModel();
                string printLn = MoneraTerminal.GetReceipt();
                moneraReceipt.Parse(printLn);

                //string sReceipt = "TID=02439406|ATH=227690  |RETNUM=001|RETTXT=ELFOGADVA|AMT=9,00|DATE=2023.04.28 23:07:08|" + 
                //    "CNB=478738XXXXXX1811|REFNO=18|ACQ=OTP BANK|CTYP=Visa Card|LOC=VECSE'S FO\" UTCA 195|MERCN=GAME RENTACAR KFT.|" + 
                //    "OWN=GAME RENTACAR|AID=A0000000031010|TC=49EF7905F7150AE2|TRID=DEID_77092";
                //moneraReceipt.Parse(sReceipt);
                int amount = (int)Double.Parse(moneraReceipt.Amount);

                Log.Info("Printing deposit receipt");
                if (model.Nyelv == Nyelvek.hu)
                {
                    PrinterFunctions.PrintReceiptHun(model.Id.ToString(), model.Rendszam, model.VegeDatum, amount, moneraReceipt.AuthCode);
                }
                else
                {
                    PrinterFunctions.PrintReceiptEng(model.Id.ToString(), model.Rendszam, model.VegeDatum, amount, moneraReceipt.AuthCode);
                }

                int authCode;
                int.TryParse(Regex.Replace(moneraReceipt.AuthCode, @"[^\d]", ""), out authCode); // Csak számokat tartalmazzon

                BookingFunctionsInst.UjCsomag(new DeliveryModel()
                {
                    OrderId = model.Id,
                    ValueInt = authCode,
                    ValueStr = printLn,
                    ValueNyelv = model.Nyelv,
                    Type = DeliveryTypes.Deposit
                });

                if (model.Fizetendo == 0) // Ha a fizetés már rendezve lett
                {
                    BookingFunctionsInst.UjCsomag(new DeliveryModel()
                    {
                        OrderId = model.Id,
                        ValueInt = 1,
                        ValueStr = "",
                        ValueNyelv = model.Nyelv,
                        Type = DeliveryTypes.Payment
                    });
                }

            }
            else
            {
                model.ZarolasMegszakadtFl = true;
                BookingFunctions.UjFoglalasVagyModositas(model);
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

            return Json(new ResultModel() { Id = res, Text = MoneraTerminalFunctions.GetErrorName(res) });
            //return Json(new ResultModel() { Id = 0, Text = "" });
        }

        [HttpPost]
        public JsonResult FizetesRendben(int id)
        {
            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            if (model.FizetesMegszakadtFl)
            {
                Log.Info($"FizetesRendben action: FizetesMegszakadtFl miatt pos újraindítás ({model.Id}).");

                posThread = null;

                MoneraTerminalFunctions.SendBreak(); 

                posThread = new Thread(
                       () => FizetesFolyamat(model));
                posThread.Start();
                
                return Json(new ResultModel() { Id = -1, Text = "Pos újraindítása" });
            }

            //if (model.FizetveFl)
            //{
            //    // Opening compartment
            //    Log.Info($"Opening Compartment: {model.RekeszId}");
            //    KerongLockFunctions.OpenCompartment((byte)model.RekeszId);
            //}

            return Json(new ResultModel() { Id = (!model.FizetveFl).GetHashCode(), Text = "" });
        }

        [HttpPost]
        public JsonResult LetetZarolasRendben(int id)
        {
            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            if (model.ZarolasMegszakadtFl)
            {
                Log.Info($"LetetZarolasRendben action: ZarolasMegszakadtFl miatt pos újraindítás ({model.Id}).");

                posThread = null;

                MoneraTerminalFunctions.SendBreak();

                posThread = new Thread(
                       () => LetetZarolasFolyamat(model));
                posThread.Start();

                return Json(new ResultModel() { Id = -1, Text = "Pos újraindítása" });
            }
            return Json(new ResultModel() { Id = (!model.ZarolvaFl).GetHashCode(), Text = "" });
        }

        [HttpPost]
        public JsonResult RevertPayment()
        {
            Log.Info("RevertPayment started");

            MoneraTerminal = new MoneraTerminalFunctions();
            MoneraTerminal.Init();
            int res = MoneraTerminal.CancelPayment();

            return Json(new ResultModel() { Id = res, Text = MoneraTerminalFunctions.GetErrorName(res) });
        }

        [HttpPost]
        public JsonResult StopPayment()
        {
            Log.Info("StopPayment started");
            int res = MoneraTerminalFunctions.SendBreak();

            return Json(new ResultModel() { Id = res, Text = MoneraTerminalFunctions.GetErrorName(res) });
        }

        [HttpPost]
        public JsonResult DailyClose()
        {
            Log.Info("DailyClose started");
            if (MoneraTerminalFunctions.DailyTask())
            {
                Log.Info("DailyClose finished");
                return Json(new ResultModel() { Id = 0, Text = "DailyClose finished" });
            }
            return Json(new ResultModel() { Id = -1, Text = "System might busy, Daily close failed" });
        }
    }
}
