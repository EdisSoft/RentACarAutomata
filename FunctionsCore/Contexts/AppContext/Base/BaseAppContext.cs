using FunctionsCore.Commons.EntitiesJson;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Enums;
using FunctionsCore.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace FunctionsCore.Contexts
{
    public class BaseAppContext : AppContextInstanceManager
    {
        public static BaseAppContext Instance => (BaseAppContext)GetInstance(typeof(BaseAppContext));


        private HttpContext httpContext;
        public HttpContext CurrentHttpContext => httpContext ?? Fonix3HttpContextAccessor.CurrentHttpContext;

        public virtual ISession CurrentSession()
        {
            
            if (!CurrentHttpContext.Session.Get<bool>("ExistsSession"))
            {
                // windows authentication
                if (CurrentHttpContext.User.Identity.AuthenticationType == "Negotiate")
                {
                    throw new NotImplementedException("Socket miatt nem használjuk sehol");
                    //var userName = GetWindowsUserName();
                    //new SessionFunctions().InitSession(userName);
                }
                // JWT token form authentication
                else
                {
                    //var tokenFunction = new TokenAuthenticationFunctions();
                    //if (AppSettingsBase.GetTokenBeallitasok() != null && tokenFunction.ValidateToken(tokenFunction.GetToken(), out var user))
                    //{
                    //    if (Enum.TryParse(user.Modul, out Modulok m))
                    //        new SessionFunctions().InitSession(user.UserName, m);
                    //    else
                    //        new SessionFunctions().InitSession(user.UserName, null);
                    //}
                }

            }
            return CurrentHttpContext.Session;
            
        }

        //public string GetWindowsUserName()
        //{
        //    var adKezeloFunctions = new AdKezeloFunctions();
        //    var identity = Fonix3HttpContextAccessor.CurrentHttpContext.User.Identity as WindowsIdentity;
        //    var user = adKezeloFunctions.GetUserFromSid(identity.User.Value);
        //    return user.SamAccountName;
        //}

        public bool ExistsSession
        {
            get => CurrentHttpContext.Session.Get<bool>("ExistsSession");
            set => CurrentHttpContext.Session.Set("ExistsSession", value);
        }

        public IHostingEnvironment HostingEnvironment { get; set; }

        public void SetCurrentHttpContext(HttpContext context)
        {
            httpContext = context;
        } 
    }


}
