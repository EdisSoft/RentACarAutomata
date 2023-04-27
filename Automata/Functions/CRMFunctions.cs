using FunctionsCore.Models;
using FunctionsCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automata.Functions
{
    public class CRMFunctions : ICRMFunctions
    {
        private IHTTPRequestService requestService;

        public CRMFunctions(IHTTPRequestService requestService)
        {
            this.requestService = requestService;
        }

        public async Task<List<FoglalasokModel>> GetFoglalasok(string nev)
        {
            return await requestService.GetFoglalasok(nev);           
        }
    }
}
