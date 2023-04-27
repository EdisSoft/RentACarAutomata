using FunctionsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsCore.Services
{
    public interface IHTTPRequestService
    {
        Task<List<FoglalasokModel>> GetFoglalasok(string nev);
    }
}
