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
        if(result != null)
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
            Type = FunctionsCore.Enums.DeliveryTypes.Email,
            SendedFl = false
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult SaveAlairas(int id, string pic)
    {
        bookingFunctions.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = pic,
            Type = FunctionsCore.Enums.DeliveryTypes.Signature,
            SendedFl = false
        });

        var nyelv = Request.Headers["Accept-Language"];

        BookingFunctions.UpdateFoglalas(id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    public JsonResult SikeresFoglalas(int id, string nyelv)
    {
        BookingFunctions.SikeresFoglalas(id, nyelv);
        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
