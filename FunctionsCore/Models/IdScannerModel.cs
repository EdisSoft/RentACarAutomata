using System;

namespace FunctionsCore.Models
{
    public class IdScannerModel
    {
        public byte[] Image { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? Validity { get; set; }
    }
}
