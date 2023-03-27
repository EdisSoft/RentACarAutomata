using FunctionsCore.Utilities.Extension.StringExtension;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FunctionsCore.Utilities.Extension
{
    public static class HttpContextExtensions
    {
        public static string GetHttpContextParameterJsonString(this HttpContext context)
        {            
            var result = new Dictionary<string, object>();
            result.Add("IpAddress", context.Connection.RemoteIpAddress.ToString());
            if (context.Request.HasFormContentType && context.Request.Form != null)
                foreach (string item in context.Request.Form.Keys)
                {
                    if (context.Request.Form[item].ToString().StartsWith("data:image/jpeg;base64,"))
                        continue;
                    if (item.ToLower().Contains("password") || item.ToLower().Contains("pinkod"))
                        continue;
                    if (item.Contains("[]"))
                        result.Add(item.FirstCharToUpper().Replace("[]", ""), context.Request.Form[item]);
                    else
                        result.Add(item.FirstCharToUpper(), (string)context.Request.Form[item]);
                }

            foreach (string item in context.Request.Query.Keys)
            {
                if (item.ToLower().Contains("password") || item.ToLower().Contains("pinkod"))
                    continue;
                if (item.Contains("[]"))
                    result.Add(item.FirstCharToUpper().Replace("[]", ""), context.Request.Query[item]);
                else
                    result.Add(item.FirstCharToUpper(), (string)context.Request.Query[item]);
            }

            if (context.Request.Body.CanSeek)
            { 
                var bodyStr = "";
                var req = context.Request;
                req.Body.Position = 0;
                req.EnableBuffering();
                
                using (StreamReader reader = new StreamReader(req.Body))
                {
                    bodyStr = reader.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(bodyStr)) 
                {
                    JObject jObject = JObject.Parse(bodyStr);
                    jObject.Descendants()
                           .OfType<JProperty>()
                           .Where(attr => attr.Name.ToLower().StartsWith("password") || attr.Name.ToLower().StartsWith("pinkod") || attr.Value.ToString().StartsWith("data:image/jpeg;base64,"))
                           .ToList()
                           .ForEach(attr => attr.Remove());

                    return jObject.ToString(Formatting.None);
                }
            }

            return result.Any() ? JsonConvert.SerializeObject(result) : null;
        }

        public static string GetHttpContextParameterJsonStringWithoutIp(this HttpContext context)
        {
            var result = new Dictionary<string, object>();            
            if (context.Request.HasFormContentType && context.Request.Form != null)
                foreach (string item in context.Request.Form.Keys)
                {
                    if (item.ToLower().Contains("password") || item.ToLower().Contains("pinkod"))
                        continue;
                    if (item.Contains("[]"))
                        result.Add(item.FirstCharToUpper().Replace("[]", ""), context.Request.Form[item]);
                    else
                        result.Add(item.FirstCharToUpper(), (string)context.Request.Form[item]);
                }

            foreach (string item in context.Request.Query.Keys)
            {
                if (item.ToLower().Contains("password") || item.ToLower().Contains("pinkod"))
                    continue;
                if (item.Contains("[]"))
                    result.Add(item.FirstCharToUpper().Replace("[]", ""), context.Request.Form[item]);
                else
                    result.Add(item.FirstCharToUpper(), (string)context.Request.Form[item]);
            }
            return result.Any() ? JsonConvert.SerializeObject(result) : null;
        }

        public static string GetHttpContextParameterValueFromForm(this HttpContext context, string parameter)
        {
            string result = "";
            if (context.Request.HasFormContentType && context.Request.Form != null)
            {
                foreach (string item in context.Request.Form.Keys)
                {
                    if (item.ToLower().Contains(parameter.ToLower()))
                    {
                        result = context.Request.Form[item].ToString();
                        break;
                    }
                }
            }
        
            return result;
        }
    }
}
