using FunctionsCore.Attributes;
using System;

namespace FunctionsCore.Commons.EntitiesJson
{
    [JsonEntity]
    public class RekordNaploJson
    {
        public DateTime UtolsoModositasDatuma { get; set; }
        //public DateTime? ForrasModositasDatuma { get; set; }
        public int UtolsoModositoBaratId { get; set; }
        //public int? FanyTranzakcioId { get; set; }
    }
}