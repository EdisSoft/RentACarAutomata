using System;

namespace FunctionsCore.Models
{
    public static class QrCodeReaderModel
    {
        private static int _code { get; set; }
        private static DateTime receivedTime { get; set; }

        public static int Code
        {
            get
            {
                TimeSpan ts = DateTime.Now - receivedTime;
                if (ts.Seconds < 3)
                    return _code;
                return 0;
            }
            set
            {
                _code = value;
                receivedTime = DateTime.Now;
            }
        }
    }
}
