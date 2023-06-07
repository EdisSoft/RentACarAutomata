using System.Collections.Generic;

namespace FunctionsCore.Models
{
    public class AutoLeadasModel
    {
        public int Id { get; set; }
        public string Rendszam { get; set; }
        public string RfId { get; set; }
        public List<int> RekeszIds { get; set; }
    }
}
