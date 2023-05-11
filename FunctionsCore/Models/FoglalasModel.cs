using FunctionsCore.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FunctionsCore.Models
{
    public class FoglalasModel
    {
        [Key]
        public int Id { get; set; }
        public string Nev { get; set; }
        public DateTime KezdDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public string Email { get; set; }
        public bool EmailFl
        {
            get
            {
                return string.IsNullOrWhiteSpace(Email) ? false : true;
            }
        }
        public int Zarolando { get; set; }
        public int Fizetendo { get; set; }
        public string Tipus { get; set; }
        public Nyelvek Nyelv { get; set; }
        public bool MindenElkuldveFl { get; set; }
        public int UtolsoVarazsloLepes { get; set; }
        public bool IdeiglenesFl { get; set; } = false;

        public int RekeszId { get; set; }

    }
}