﻿using Automata.Functions;
using FunctionsCore;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Enums;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICrmFunctions CrmFunctions { get; set; }

    private IBookingFunctions BookingFunctionsInst { get; set; }

    private IIdScannerFunctions IdScannerFunctions { get; set; }

    public FoglalasController(ICrmFunctions crmFunctions, IBookingFunctions bookingFunctions, IIdScannerFunctions idScannerFunctions)
    {
        CrmFunctions = crmFunctions;
        BookingFunctionsInst = bookingFunctions;
        IdScannerFunctions = idScannerFunctions;
    }

    [HttpPost]
    public async Task<JsonResult> GetFoglalasok(string nev)
    {
        var result = await CrmFunctions.GetFoglalasokByNev(nev);
        BookingFunctions.UjFoglalas(result);

        return Json(result);
    }

    public async Task<JsonResult> ReadQr()
    {
        var result = await CrmFunctions.GetFoglalasByQrCode();
        if (result != null)
        {
            BookingFunctions.UjFoglalasVagyModositas(result);
        }

        return Json(result);
    }

    [HttpPost]
    public JsonResult SaveEmail(int id, string email)
    {
        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = email,
            Type = DeliveryTypes.Email
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult SaveAlairas([FromBody] AlairasModel model)
    {
        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = model.Id,
            ValueStr = model.Pic,
            Type = DeliveryTypes.Signature
        });

        var nyelv = Request.Headers["Accept-Language"];

        BookingFunctions.UpdateFoglalas(model.Id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceFront(int id)
    {
        var model = IdScannerFunctions.ScanCard();
        
        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanLicenceFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 6); // 5+1

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanLicenceBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardFrontOrPassport(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanIdCardFrontOrPassport
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 7); //6+1

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanIdCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardFront(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanCreditCardFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 8); // 7+1

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanCreditCardBack
        });

        //FoglalasModel foglalas = null;
        if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel foglalas))
        {
            throw new Exception($"ScanCreditCardBack: No such reservation: {id}");
        }

        if (foglalas.Zarolando == 0)
        {
            BookingFunctionsInst.UjCsomag(new DeliveryModel()  // Ha nem kell deposit
            {
                OrderId = foglalas.Id,
                ValueInt = 0,
                ValueStr = "",
                ValueNyelv = foglalas.Nyelv,
                Type = DeliveryTypes.Deposit
            });

            if (foglalas.Fizetendo == 0)  // Ha a fizetés is már rendezve lett – különben a deposit részen figyeljük
            {
                BookingFunctionsInst.UjCsomag(new DeliveryModel()
                {
                    OrderId = foglalas.Id,
                    ValueInt = 0,
                    ValueStr = "",
                    ValueNyelv = foglalas.Nyelv,
                    Type = DeliveryTypes.Payment
                });
            }
        }


        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public async Task<JsonResult> Leadas(string rendszam)
    {
        var result = await CrmFunctions.KocsiLeadas(rendszam);

        if (result != null)
        {
            int? rekeszId = BookingFunctionsInst.SetTempValues(result.Id, result.RekeszIds);

            if (rekeszId == null)
                throw new WarningException("There is no free slot.", WarningExceptionLevel.Warning);

            return Json(new { Id = result.Id, RekeszId = rekeszId });
        }
        throw new WarningException("No car with this license plate has been issued.", WarningExceptionLevel.Warning);
    }

    [HttpPost]
    public JsonResult KulcsLeadas(int id, bool taxiFl)
    {
        var rekeszIdOriginal = BookingFunctionsInst.GetRekeszId(id);        

        var result = CrmFunctions.KulcsLeadas(id, rekeszIdOriginal, taxiFl);

        return Json(new ResultModel() { Id = (!result.Result).GetHashCode(), Text = "" });
    }
}
