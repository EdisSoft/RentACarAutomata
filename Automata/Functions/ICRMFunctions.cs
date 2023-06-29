using FunctionsCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automata.Functions
{
    public interface ICrmFunctions
    {
        Task<List<FoglalasModel>> GetFoglalasokByNev(string nev);
        Task<FoglalasModel> GetFoglalasByQrCode();
        Task<AutoLeadasModel> KocsiLeadas(string rendszam);
        Task<bool> KulcsLeadas(int id, byte rekeszId, bool taxiFl);
    }
}
