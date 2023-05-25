using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICrmFunctions CrmFunctions { get; set; }

    private IBookingFunctions BookingFunctions { get; set; }

    private IdScannerFunctions IdScannerFunctions { get; set; }

    public FoglalasController(ICrmFunctions crmFunctions, IBookingFunctions bookingFunctions)
    {
        CrmFunctions = crmFunctions;
        BookingFunctions = bookingFunctions;

        IdScannerFunctions = new IdScannerFunctions();
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

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public async Task<JsonResult> Leadas(string rendszam)
    {
        var result = await CrmFunctions.KocsiLeadas(rendszam);

        BookingFunctions.SetTempValues(result.Id, result.LockNumber);

        return Json(result.LockNumber);
    }

    [HttpPost]
    public JsonResult KulcsLeadas(int id, bool taxiFl)
    {
        var rekeszId = BookingFunctions.GetRekeszId(id);        

        CrmFunctions.KulcsLeadas(id, rekeszId, taxiFl);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
