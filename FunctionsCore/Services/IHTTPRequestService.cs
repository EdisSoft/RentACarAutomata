using FunctionsCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionsCore.Services
{
    public interface IHttpRequestService
    {
        Task<List<FoglalasModel>> GetFoglalasokByNev(string nev);
        Task<FoglalasModel> GetFoglalasByCode(string code);
        Task<bool>SaveEmail(int id, string email);
        Task<bool>SaveSignature(int id, string signature);
        Task<bool>SendDeposit(int id, string language, int deposittrid, string slip);
        Task<bool> SendPayment(int id, string language, int paymenttrid, string slip);
        Task<AutoLeadasModel> KocsiLeadas(string rendszam);
        Task<bool> KulcsLeadas(int id, int rekeszId, bool taxiFl);
    }
}
