using FunctionsCore.Attributes;
using FunctionsCore.Contexts;
using FunctionsCore.Utilities.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Automata.Controllers
{
    //[Authorize]
    [TrackTimeFilter]
    public class BaseController : Controller
    {
        protected string GetCookie(string key)
        {
            return Request.Cookies[key];
        }

        protected void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);

            Response.Cookies.Append(key, value, option);
        }
        public JsonResult GetVersionInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            var fileInfo = new FileInfo(assembly.Location);
            var buildDate = fileInfo.LastWriteTime;
            return Json(new { version, date = buildDate });
        }

        protected void RemoveCookie(string key)
        {
            Response.Cookies.Delete(key);
        }
        public JsonResult EmptyJson()
        {
            return Json(new EmptyResult());
        }

        public static bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#endif
#if !DEBUG
                return false;
#endif

            }
        }
    }
}
