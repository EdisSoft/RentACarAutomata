using FunctionsCore.Commons.Entities.Base;
using FunctionsCore.Commons.EntitiesJson;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.Entities
{
    [Table("Cimkek")]
    public class Cimke : ExtendedEntity
    {
        public FelhoJson Felho { get; set; }
        public string Nev { get; set; }
        public string RovidNev { get; set; }
        public int? FanyId { get; set; }
        public int? Fonix2Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int FelhoId { get; set; }
        public string Leiras { get; set; }
        public string IkonSzine { get; set; }
        public string IkonBetu { get; set; }
        public string FanyAzonKod { get; set; }
        public string FanyKodcsopAzonKod { get; set; }
        public int? Sorrend { get; set; }
        public bool ElesenRejtettFl { get; set; }
        public bool SzinkronizalodjonFl { get; set; }
    }
}
