using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Automata.Controllers
{
    public class LockController : BaseController
    {

        //[HttpPost]
        public JsonResult OpenLock(int lockNo)
        {
            KerongLockFunctions locks = new KerongLockFunctions();
            locks.OpenLock((byte)lockNo);
            return Json(new ResultModel() { Id = 0, Text = "Opening lock " + lockNo });
        }

        [HttpPost]
        public JsonResult LockStatus(int lockNo)
        {
            KerongLockFunctions locks = new KerongLockFunctions();
            if (locks.IsLockClosed((byte)lockNo))
            {
                return Json(new ResultModel() { Id = 0, Text = "Lock " + lockNo + " is closed" });
            }
            return Json(new ResultModel() { Id = 0, Text = "Lock " +lockNo + " is opened" });
        }
    }
}
