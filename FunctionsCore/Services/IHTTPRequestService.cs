using FunctionsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsCore.Services
{
    public interface IHttpRequestService
    {
        Task<List<FoglalasModel>> GetFoglalasokByNev(string nev);
        Task<FoglalasModel> GetFoglalasByCode(string code);
        void SaveEmail(int id, string email);
        void SaveSignature(int id, string signature);
        void SendDeposit(int id, string language, int deposittrid, string slip);
        void SendPayment(int id, string language, int deposittrid, string slip);
        Task<AutoLeadasModel> KocsiLeadas(string rendszam);
        Task KulcsLeadas(int id, int rekeszId, bool taxiFl);
    }
}
