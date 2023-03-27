using FunctionsCore.Commons.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.Entities
{

    [Table("Felhok")]
    public class Felho : ExtendedEntity
    {
        public string Nev { get; set; }
        public int? FanyId { get; set; }
        public int? Fonix2Id { get; set; }
        public bool FeluletrolUjCimkeAdhatoHozzaFl { get; set; }
    }
}