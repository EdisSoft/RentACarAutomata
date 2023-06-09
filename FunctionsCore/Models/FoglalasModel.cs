﻿using FunctionsCore.Enums;
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
        public string Rendszam { get; set; }
        public int Zarolando { get; set; }
        public int Fizetendo { get; set; }
        public string Tipus { get; set; }
        public Nyelvek Nyelv { get; set; }
        public int UtolsoVarazsloLepes { get; set; }
        public bool IdeiglenesFl { get; set; } = false;
        public int RekeszId { get; set; }
        public bool ZarolvaFl { get; set; }
        public bool FizetveFl { get; set; }
        public bool ZarolasMegszakadtFl { get; set; }
        public bool FizetesMegszakadtFl { get; set; }
        public bool SkipDocReadingFl { get; set; }
        public bool TorolhetoFl { get; set; }
        public DateTime UtolsoModositas { get; set; }
        public bool AktivUgyfelFl
        {
            get
            {
                return UtolsoModositas > DateTime.Now.AddMinutes(-10);
            }
        }

        public override string ToString()
        {
            return $"{{{Id}, {Nev}, Step: {UtolsoVarazsloLepes}, Nye: {Nyelv}, Rek: {RekeszId}, Zár: {Zarolando}/{ZarolvaFl}, Fiz: {Fizetendo}/{FizetveFl}, Tör: {TorolhetoFl}}}";
        }
    }
}