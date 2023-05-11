﻿using FunctionsCore;
using FunctionsCore.Models;
using FunctionsCore.Services;
using System;
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
        try
        {
            var result = await requestService.GetFoglalasokByNev(nev);
            if (result is null || result.Count == 0)
            {
                throw new FunctionsCore.WarningException("Nincs foglalás!", FunctionsCore.WarningExceptionLevel.Warning);
            }

            return result;
        }
        catch (Exception e)
        {
            throw new FunctionsCore.WarningException("Hiba történt!", FunctionsCore.WarningExceptionLevel.Warning);
        }
    }

    public async Task<FoglalasModel> GetFoglalasByQrCode()
    {
        var code = QrCodeReaderModel.Code;

        if (code == "")
        {
            return null;
        }
        Log.Info("kpok");

        return await requestService.GetFoglalasByCode(code);
    }
}
