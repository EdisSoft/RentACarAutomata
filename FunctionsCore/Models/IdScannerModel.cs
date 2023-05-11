using System;

namespace FunctionsCore.Models
{
    public class IdScannerModel
    {
        public string OkmanyTipus { get; set; }
        public int EredetisegValoszinusege { get; set; }
        public string Nev { get; set; }
        public DateTime? ErvenyessegVege { get; set; }
        public byte[] Kep { get; set; }
    }
}
