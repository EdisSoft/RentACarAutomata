using Automata.Functions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Automata.Controllers
{
    public class FoglalasController : BaseController
    {
        ICRMFunctions CRMFunctions { get; set; }

        public FoglalasController(ICRMFunctions cRMFunctions)
        {
            CRMFunctions = cRMFunctions;
        }

        public async Task<JsonResult> GetFoglalasok(string nev)
        {
            var result = await CRMFunctions.GetFoglalasok(nev);
            return Json(result);
        }
    }
}
