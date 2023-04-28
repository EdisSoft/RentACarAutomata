using FunctionsCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automata.Functions
{
    public interface ICRMFunctions
    {
        Task<List<FoglalasModel>> GetFoglalasokByNev(string nev);
        Task<FoglalasModel> GetFoglalasByQrCode();
    }
}
