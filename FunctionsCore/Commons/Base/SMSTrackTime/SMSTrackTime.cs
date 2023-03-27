using System.Collections.Generic;

namespace FunctionsCore.Commons.Base
{
    public class SmsTrackTime
    {

        public int AvgTimeLimitMs { get; set; }
        public int SendingGapMinutes { get; set; }
        public int QueueCount { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public string SmsUrl { get; set; }
    }

}

