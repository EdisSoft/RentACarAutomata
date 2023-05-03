using FunctionsCore.Models;
using FunctionsCore.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automata.Functions;

public class CRMFunctions : ICRMFunctions
{
    private IHTTPRequestService requestService;

    public CRMFunctions(IHTTPRequestService requestService)
    {
        this.requestService = requestService;
    }

    public async Task<List<FoglalasModel>> GetFoglalasokByNev(string nev)
    {
        return await requestService.GetFoglalasokByNev(nev);
    }

    public async Task<FoglalasModel> GetFoglalasByQrCode()
    {
        var code = QrCodeReaderModel.Code;

        if (code == "")
        {
            return null;
        }

        return await requestService.GetFoglalasByCode(code);
    }       
}
