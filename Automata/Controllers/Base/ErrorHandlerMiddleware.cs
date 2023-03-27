using FunctionsCore;
using FunctionsCore.Commons.Base;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Utilities.SqlHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Controllers.Base
{
    public class ErrorHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                await HandleError(context, error);
            }
        }
        public async Task HandleError(HttpContext context, Exception exception)
        {
           
            string url = context.Request.Path.ToString();

            if (!(exception is WarningException && (((WarningException)exception).WarningExceptionLevel == WarningExceptionLevel.Validation)))
            {
                StringBuilder sb = new StringBuilder();
                if (context.Request.HasFormContentType && context.Request.Form != null)
                    foreach (string item in context.Request.Form.Keys)
                    {
                        if (!item.ToLower().Contains("password"))
                        {
                            var data = context.Request.Form[item].ToString() ?? "";
                            sb.Append(item + "=" + (data.Length <= 500 ? data : data.Substring(0, 500)) + ";");
                        }
                    }

                foreach (string item in context.Request.Query.Keys)
                {
                    if (!item.ToLower().Contains("password"))
                    {
                        var data = context.Request.Query[item].ToString() ?? "";
                        sb.Append(item + "=" + (data.Length <= 500 ? data : data.Substring(0, 500)) + ";");
                    }
                }

                SaveLog(url, context.Request.Method, sb.ToString(), context.Request.HttpContext.Connection.RemoteIpAddress.ToString(), exception.StackTrace, exception);
            }
            if (exception is WarningException && (((WarningException)exception).WarningExceptionLevel == WarningExceptionLevel.Validation))
            {
                var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
               
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            if (exception != null)
            {
                var response = new WarningExceptionResponse()
                {
                    ServerError = true
                };

                if (exception is WarningException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    WarningException ex = (WarningException)exception;

                    response.ErrorLevel = ex.WarningExceptionLevel;

                    switch (ex.WarningExceptionLevel)
                    {
                        case WarningExceptionLevel.Validation:
                        case WarningExceptionLevel.Warning:
                            response.Title = "Figyelmeztetés!";
                            break;
                        case WarningExceptionLevel.Error:
                            response.Title = "Hiba!";
                            break;
                    }

                    response.Message = ex.Message;
                }
                else
                {
                    response.Title = "Hiba!";
                    response.Message = "Hiba történt az alkalmazásban!";
                    response.ErrorLevel = WarningExceptionLevel.Error;
                }

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

            }

        }

        private void SaveLog(string Where, string httpMethod, string data, string ip, string stacktrace, Exception e)
        {

            var appcontext = BaseAppContext.Instance;
            string ErrorText = $"Hely:{Where}\r\n IpAddress:{ip}\r\nhttpMethod:{httpMethod}\r\nValtozok:{data}\r\nStackTrace:{stacktrace}\r\n"
               ;
            if (e is WarningException exception && (exception.WarningExceptionLevel == WarningExceptionLevel.Warning))
            {
                Log.Warning(ErrorText, e);
            }
            else
            {
                Log.Error(ErrorText, e);
            }
        }
    }
}
