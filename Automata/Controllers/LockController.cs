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
        public JsonResult OpenLock(int rekeszId)
        {
            KerongLockFunctions.OpenLock((byte)rekeszId);
            return Json(new ResultModel() { Id = 0, Text = "Opening lock " + rekeszId });
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
        public JsonResult OpenCompartment(int compNo)
        {
            Log.Debug($"Lock/OpenCompartment({compNo})");

            KerongLockFunctions.OpenCompartment((byte)compNo);
            return Json(new ResultModel() { Id = 0, Text = "Opening compartment " + compNo });
        }

        [HttpPost]
        public JsonResult CompartmentStatus(int compNo)
        {
            if (KerongLockFunctions.IsCompartmentClosed((byte)compNo))
            {
                return Json(new ResultModel() { Id = 0, Text = "Compartment " + compNo + " is closed" });
            }
            return Json(new ResultModel() { Id = 0, Text = "Compartment " + compNo + " is opened" });
        }

        [HttpPost]
        public JsonResult CompartmentStatuses()
        {
            var model = KerongLockFunctions.GetCompartmentStatuses();
            return Json(model);
        }

        [HttpPost]
        public JsonResult OpenLockByBookingId(int id)
        {
            Log.Debug($"Lock/OpenLockByBookingId({id})");

            if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel model))
            {
                throw new Exception("No such reservation");
            }

            KerongLockFunctions.OpenCompartment((byte)model.RekeszId);

            return Json(new ResultModel() { Id = 0, Text = model.RekeszId.ToString() });
        }
    }
}
