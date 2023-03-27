using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.Entities
{
    public class BaseEntity : SoftDeleteEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Csabával megbeszélni
        //public DateTime ErvenyessegKezdete { get; set; }
    }
}
