using FunctionsCore.Enums;
using System;

namespace FunctionsCore.Models
{
    public class OkmanyModel
    {
        public DocumentTypes Tipus { get; set; }
        public int EredetisegValoszinusege { get; set; }
        public string Nev { get; set; }
        public DateTime? ErvenyessegVege { get; set; }
        public byte[] Kep { get; set; }
    }
}
