using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace FunctionsCore
{
    public class WarningExceptionResponse
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool ServerError { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public WarningExceptionLevel ErrorLevel { get; set; }
        public int ErrorLevelCode
        {
            get
            {
                return (int)ErrorLevel;
            }
        }
    }
}
