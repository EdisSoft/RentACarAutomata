using FunctionsCore.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionsCore.Commons.EntitiesJson
{
    [JsonEntity]
    public class FelhoJson
    {
        public int Id { get; set; }
        public string Nev { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Nev}";
        }

    }
}