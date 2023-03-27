namespace FunctionsCore.Commons.Base
{
    public class TrackTimeFilter
    {
        public int TrackTimeLog { get; set; }
        public int TrackCountLog { get; set; }

        public bool EnableRequestLog { get; set; }

        public int RequestLogInterval { get; set; }
    }
}
