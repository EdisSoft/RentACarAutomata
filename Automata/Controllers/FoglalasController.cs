using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICrmFunctions crmFunctions { get; set; }

    private IBookingFunctions bookingFunctions { get; set; }

    public FoglalasController(ICrmFunctions crmFunctions, IBookingFunctions bookingFunctions)
    {
        this.crmFunctions = crmFunctions;
        this.bookingFunctions = bookingFunctions;
    }

    [HttpGet]
    public async Task<JsonResult> GetFoglalasok(string nev)
    {
        var result = await crmFunctions.GetFoglalasokByNev(nev);
        BookingFunctions.UjFoglalas(result);

        return Json(result);
    }

    public async Task<JsonResult> ReadQr()
    {
        var result = await crmFunctions.GetFoglalasByQrCode();
        if (result != null)
        {
            BookingFunctions.UjFoglalas(result);
        }

        return Json(result);
    }

    [HttpPost]
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
    public JsonResult SaveAlairas([FromBody] AlairasModel model)
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

    [HttpPost]
    public JsonResult ScanLicenceFront(int id,[FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 5);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceBack(int id, [FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanLicenceBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardFrontOrPassport(int id, [FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardFrontOrPassport
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 6);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardBack(int id, [FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanIdCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardFront(int id, [FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 7);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardBack(int id, [FromBody] byte[] picture)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = picture,
            Type = FunctionsCore.Enums.DeliveryTypes.ScanCreditCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
