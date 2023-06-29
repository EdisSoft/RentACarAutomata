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
                throw new WarningException(""); // Nincs foglalás!
            }

            if(result.Count == 1)
            {
                result[0].IdeiglenesFl = false;
            }

            return result;
        }
        catch (Exception e)
        {
            if (e is WarningException)
                throw new WarningException("No such reservation."); // Nincs foglalás!
            throw new WarningException("An error occurred during processing.");
        }
    }

    public async Task<FoglalasModel> GetFoglalasByQrCode()
    {
        var code = QrCodeReaderModel.Code;

        if (code == null)
        {
            return null;
        }

        return await requestService.GetFoglalasByCode(code);
    }

    public async Task<AutoLeadasModel> KocsiLeadas(string rendszam)
    {       
        return await requestService.KocsiLeadas(rendszam);
    }

    public async Task<bool> KulcsLeadas(int id, byte rekeszId, bool taxiFl)
    {
       return await requestService.KulcsLeadas(id, rekeszId, taxiFl);
    }   
}
