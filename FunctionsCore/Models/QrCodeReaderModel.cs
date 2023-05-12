using System;

namespace FunctionsCore.Models
{
    public static class QrCodeReaderModel
    {
        private static string _code { get; set; }
        private static DateTime receivedTime { get; set; }

        public static string Code
        {
            get
            {
                TimeSpan ts = DateTime.Now - receivedTime;
                if (ts.Seconds < 3)
                    return _code;
                return null;
            }
            set
            {
                _code = value;
                receivedTime = DateTime.Now;
            }
        }
    }
}
