using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers
{
    public class FoglalasController : BaseController
    {
        private ICRMFunctions CRMFunctions { get; set; }

        private IDeliveryFunctions deliveryFunctions { get; set; }

        public FoglalasController(ICRMFunctions CRMFunctions, IDeliveryFunctions deliveryFunctions)
        {
            this.CRMFunctions = CRMFunctions;
            this.deliveryFunctions = deliveryFunctions;
        }

        public async Task<JsonResult> GetFoglalasok(string nev)
        {
            var result = await CRMFunctions.GetFoglalasokByNev(nev);
            return Json(result);
        }

        public async Task<JsonResult> IsQRCode()
        {
            var result = await CRMFunctions.GetFoglalasByQrCode();
            return Json(result);
        }
        
        public void SaveEmail(int id, string email)
        {
            deliveryFunctions.UjCsomag(new DeliveryModel()
            {
                OrderId = id,
                ValueStr = email,
                Type = FunctionsCore.Enums.DeliveryTypes.Email,
                SendedFl = false
            });
        }

        public void SaveAlairas(int id, string pic)
        {
            deliveryFunctions.UjCsomag(new DeliveryModel()
            {
                OrderId = id,
                ValueStr = pic,
                Type = FunctionsCore.Enums.DeliveryTypes.Signature,
                SendedFl = false
            });
        }
    }
}
