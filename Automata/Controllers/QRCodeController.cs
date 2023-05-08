using Automata.Functions;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Automata.Controllers
{
    public class QrCodeController : BaseController
    {

        //[HttpPost]
        public JsonResult ReadQr()
        {
            return Json(new ResultModel() { Id = 0, Text = QrCodeReaderModel.Code });
        }

        [HttpPost]
        public JsonResult Start()
        {
            return Json(new ResultModel() { Id = 0, Text = "" });
        }
    }
}
