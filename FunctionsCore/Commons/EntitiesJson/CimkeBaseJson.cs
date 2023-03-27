using FunctionsCore.Attributes;

namespace FunctionsCore.Commons.EntitiesJson
{
    [JsonEntity]
    public class CimkeBaseJson
    {
        public int Id { get; set; }
        public string Nev { get; set; }
        public string RovidNev { get; set; }

        public override string ToString()
        {
            return $"{Id} - {Nev}";
        }
    }
}