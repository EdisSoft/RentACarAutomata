using System.Collections.Generic;

namespace FunctionsCore.Commons.Base
{
    public class AdSettings
    {
        public string AdPort { get; set; }
        public string AdFelhasznaloNev { get; set; }
        public string AdJelszo { get; set; }
        public string AdDebugGyokerUt { get; set; }

        public List<string> AdDomainControllers { get; set; }
    }
}
