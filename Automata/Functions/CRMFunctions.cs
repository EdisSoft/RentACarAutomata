using FunctionsCore;
using FunctionsCore.Models;
using FunctionsCore.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automata.Functions;

public class CrmFunctions : ICrmFunctions
{
    private IHttpRequestService requestService;

    public CrmFunctions(IHttpRequestService requestService)
    {
        this.requestService = requestService;
    }

    public async Task<List<FoglalasModel>> GetFoglalasokByNev(string nev)
    {
        try
        {
            var result = await requestService.GetFoglalasokByNev(nev);
            if (result is null || result.Count == 0)
            {
                throw new WarningException("Nincs foglalás!", WarningExceptionLevel.Warning);
            }

            if(result.Count == 1)
            {
                result[0].IdeiglenesFl = false;
            }

            return result;
        }
        catch
        {
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
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
