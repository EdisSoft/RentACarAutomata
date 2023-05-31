using Automata.Functions;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using NLog.Fluent;
using System;

namespace Automata.Controllers
{
    public class LockController : BaseController
    {

        private IKerongLockFunctions KerongLockFunctions { get; set; }

        public LockController(IKerongLockFunctions kerongLockFunctions)
        {
            KerongLockFunctions = kerongLockFunctions;
        }

        [HttpPost]
        public JsonResult OpenLock(int lockNo)
        {
            KerongLockFunctions.OpenLock((byte)lockNo);
            return Json(new ResultModel() { Id = 0, Text = "Opening lock " + lockNo });
        }

        [HttpPost]
        public JsonResult LockStatus(int lockNo)
        {
            if (KerongLockFunctions.IsLockClosed((byte)lockNo))
            {
                return Json(new ResultModel() { Id = 0, Text = "Lock " + lockNo + " is closed" });
            }
            return Json(new ResultModel() { Id = 0, Text = "Lock " +lockNo + " is opened" });
        }

        [HttpPost]
        public JsonResult OpenLockByBookingId(int id)
        {
            Log.Debug($"Lock/OpenLockByBookingId({id})");

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            KerongLockFunctions.OpenLock((byte)model.RekeszId);

            return Json(new ResultModel() { Id = 0, Text = model.RekeszId.ToString() });
        }
    }
}
