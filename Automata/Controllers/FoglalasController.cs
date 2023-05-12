using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICrmFunctions CrmFunctions { get; set; }

    private IBookingFunctions bookingFunctions { get; set; }

    public FoglalasController(ICrmFunctions CRMFunctions, IBookingFunctions bookingFunctions)
    {
        this.CrmFunctions = CRMFunctions;
        this.bookingFunctions = bookingFunctions;
    }

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

    [HttpPost]
    public JsonResult SaveAlairas([FromBody]AlairasModel model)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = model.Id,
            ValueStr = model.Pic,
            Type = FunctionsCore.Enums.DeliveryTypes.Signature
        });

        var nyelv = Request.Headers["Accept-Language"];

        BookingFunctions.UpdateFoglalas(model.Id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult ScanLicenceFront(int id)
    {
        //temp
        IdScannerFunctions idScannerFunctions = new IdScannerFunctions();
        var result = idScannerFunctions.ScanCard();

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
        var result = idScannerFunctions.ScanCard();

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
        var result = idScannerFunctions.ScanCard();

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
        var result = idScannerFunctions.ScanCard();

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
        var result = idScannerFunctions.ScanCard();

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
        var result = idScannerFunctions.ScanCard();

        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = result.Kep,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
