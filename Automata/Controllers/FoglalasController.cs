using Automata.Functions;
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

    private IBookingFunctions BookingFunctions { get; set; }

    private IIdScannerFunctions IdScannerFunctions { get; set; }

    public FoglalasController(ICrmFunctions crmFunctions, IBookingFunctions bookingFunctions, IIdScannerFunctions idScannerFunctions)
    {
        CrmFunctions = crmFunctions;
        BookingFunctions = bookingFunctions;
        IdScannerFunctions = idScannerFunctions;
    }

    [HttpPost]
    public async Task<JsonResult> GetFoglalasok(string nev)
    {
        var result = await CrmFunctions.GetFoglalasokByNev(nev);
        FunctionsCore.Commons.Functions.BookingFunctions.UjFoglalas(result);

        return Json(result);
    }

    public async Task<JsonResult> ReadQr()
    {
        var result = await CrmFunctions.GetFoglalasByQrCode();
        if (result != null)
        {
            FunctionsCore.Commons.Functions.BookingFunctions.UjFoglalas(result);
        }

        return Json(result);
    }

    [HttpPost]
    public JsonResult SaveEmail(int id, string email)
    {
        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = email,
            Type = FunctionsCore.Enums.DeliveryTypes.Email
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult SaveAlairas([FromBody] AlairasModel model)
    {
        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = model.Id,
            ValueStr = model.Pic,
            Type = FunctionsCore.Enums.DeliveryTypes.Signature
        });

        var nyelv = Request.Headers["Accept-Language"];

        FunctionsCore.Commons.Functions.BookingFunctions.UpdateFoglalas(model.Id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceFront(int id)
    {
        var model = IdScannerFunctions.ScanCard();
        
        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceFront
        });

        FunctionsCore.Commons.Functions.BookingFunctions.UpdateUtolsoVarazsloLepes(id, 5);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardFrontOrPassport(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardFrontOrPassport
        });

        FunctionsCore.Commons.Functions.BookingFunctions.UpdateUtolsoVarazsloLepes(id, 6);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardFront(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardFront
        });

        FunctionsCore.Commons.Functions.BookingFunctions.UpdateUtolsoVarazsloLepes(id, 7);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardBack(int id)
    {
        var model = IdScannerFunctions.ScanCard();

        BookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardBack
        });

        //FoglalasModel foglalas = null;
        if (!FunctionsCore.Commons.Functions.BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel foglalas))
        {
            throw new Exception($"ScanCreditCardBack: No such reservation: {id}");
        }

        if (foglalas.Zarolando == 0)
        {
            BookingFunctions.UjCsomag(new DeliveryModel()  // Ha nem kell deposit
            {
                OrderId = foglalas.Id,
                ValueInt = 0,
                ValueStr = "",
                ValueNyelv = foglalas.Nyelv,
                Type = DeliveryTypes.Deposit
            });

            if (foglalas.Fizetendo == 0)  // Ha a fizetés is már rendezve lett – különben a deposit részen figyeljük
            {
                BookingFunctions.UjCsomag(new DeliveryModel()
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
            int? rekeszId = BookingFunctions.SetTempValues(result.Id, result.RekeszIds);

            if (rekeszId == null)
                throw new WarningException("There is no free slot.", WarningExceptionLevel.Warning);

            return Json(new { Id = result.Id, RekeszId = rekeszId });
        }
        throw new WarningException("No car with this license plate has been issued.", WarningExceptionLevel.Warning);
    }

    [HttpPost]
    public JsonResult KulcsLeadas(int id, bool taxiFl)
    {
        var rekeszIdOriginal = BookingFunctions.GetRekeszId(id);        

        CrmFunctions.KulcsLeadas(id, rekeszIdOriginal, taxiFl);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
