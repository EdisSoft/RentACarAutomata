using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Automata.Controllers
{
    public class LockController : BaseController
    {

        //[HttpPost]
        public JsonResult OpenLock(int lockno)
        {
            KerongLockFunctions locks = new KerongLockFunctions();
            locks.OpenLock((byte)lockno);
            return Json(new ResultModel() { Id = 0, Text = "Opening lock " + lockno });
        }

        [HttpPost]
        public JsonResult LockStatus(int lockno)
        {
            KerongLockFunctions locks = new KerongLockFunctions();
            if (locks.IsLockClosed((byte)lockno))
            {
                return Json(new ResultModel() { Id = 0, Text = "Lock " + lockno + " is closed" });
            }
            return Json(new ResultModel() { Id = 0, Text = "Lock " +lockno + " is opened" });
        }
    }
}
