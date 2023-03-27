using FunctionsCore.Commons.EntitiesJson;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.Entities
{
    [Table("Aktivitasok")]
    public class Aktivitas : BaseEntity
    {
        public DateTime LetrehozasDatuma { get; set; } = DateTime.Now;
        public DateTime Datum { get; set; }
        public int ModulCid { get; set; }
        public int TevekenysegCid { get; set; }
        [Column(TypeName = "BIT")]
        public bool ModositasFl { get; set; }
        public int? SzemelyId { get; set; }
        public int? BaratId { get; set; }
        public int? IntezetId { get; set; }
        public string TovabbiInfo { get; set; }
        public string Osszegzes { get; set; }
        public string GEO { get; set; }
        public RekordNaploJson RekordNaplo { get; set; }
        public int? FogvatartasId { get; set; }
        public bool FeluletenLathatoFl { get; set; }
        public int? KapcsolodoEntitasId { get; set; }
        public bool? HibaFl { get; set; }
    }
}
