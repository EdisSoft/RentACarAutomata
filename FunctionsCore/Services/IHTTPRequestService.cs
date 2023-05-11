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
    }
}
