using System;
using System.Collections.Generic;

namespace FunctionsCore.Models
{
    public static class FoglalasKucsLeadasModel
    {
        static byte? _rekeszId;

        public static int Id { get; set; }
        public static List<byte> RekeszIds { get; set; }

        public static byte? RekeszId { get { return _rekeszId; } }
        public static byte? SetRekeszId()
        {
            Random random = new Random();
            int index = random.Next(RekeszIds.Count);
            _rekeszId = RekeszIds[index];
            return _rekeszId;
        }
    }
}
