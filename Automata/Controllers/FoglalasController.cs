using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICRMFunctions CRMFunctions { get; set; }

    private IBookingFunctions bookingFunctions { get; set; }

    public FoglalasController(ICRMFunctions CRMFunctions, IBookingFunctions bookingFunctions)
    {
        this.CRMFunctions = CRMFunctions;
        this.bookingFunctions = bookingFunctions;
    }

    public async Task<JsonResult> GetFoglalasok(string nev)
    {
        var result = await CRMFunctions.GetFoglalasokByNev(nev);
        BookingFunctions.UjFoglalas(result);

        return Json(result);
    }

    public async Task<JsonResult> IsQRCode()
    {
        var result = await CRMFunctions.GetFoglalasByQrCode();
        if (result != null)
        {
            BookingFunctions.UjFoglalas(result);
        }

        return Json(result);
    }

    public JsonResult SaveEmail(int id, string email)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = email,
            Type = FunctionsCore.Enums.DeliveryTypes.Email
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult SaveAlairas(int id, string pic)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = pic,
            Type = FunctionsCore.Enums.DeliveryTypes.Signature
        });

        var nyelv = Request.Headers["Accept-Language"];

        BookingFunctions.UpdateFoglalas(id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanLicenceFront(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 5);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanLicenceBack(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanIdCardFrontOrPassport(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardFrontOrPassport
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 6);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanIdCardBack(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanCreditCardFront(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 7);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanCreditCardBack(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanLicenceFront(id);

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
