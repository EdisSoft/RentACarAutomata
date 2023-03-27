using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.Entities
{
    public class SoftDeleteEntity
    {
        [Column(TypeName = "BIT")]
        public bool ToroltFl { get; set; }
    }
}
