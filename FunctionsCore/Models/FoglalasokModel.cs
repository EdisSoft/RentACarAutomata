using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsCore.Models
{
    public class FoglalasokModel
    {
        public int Id { get; set; }   
        public string Nev { get; set; }
        public DateTime KezdDatum { get; set; }
        public DateTime VegeDatum { get; set; }
        public bool EmailFl { get; set; }
    }
}
