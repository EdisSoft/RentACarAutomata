using System.Collections.Generic;

namespace FunctionsCore.Models
{
    public class CrmAutoLeadasModel
    {
        public int Id { get; set; }
        public string Rendszam { get; set; }
        public string Keyid { get; set; }
        public List<byte> LockNumbers  { get; set; }
    }
}
