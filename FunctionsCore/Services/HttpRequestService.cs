﻿using FunctionsCore.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FunctionsCore.Services;

public class HttpRequestService : IHttpRequestService
{
    private readonly HttpClient httpClient;
    private readonly CrmRequestOptions options;

    public HttpRequestService(HttpClient httpClient, IConfiguration configuration)
    {
        options = configuration.GetSection(nameof(CrmRequestOptions)).Get<CrmRequestOptions>();

        this.httpClient = httpClient;

        SetAuthorization();
    }

    private void SetAuthorization()
    {
        var authenticationString = $"{options.UserName}:{options.Password}";
        var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);
    }

    public async Task<List<FoglalasModel>> GetFoglalasokByNev(string nev)
    {
        var responseString = await httpClient.GetStringAsync(options.RequestBase + "?action=pickup&mainparam=" + nev);
        var CRMFoglalasok = JsonConvert.DeserializeObject<List<CrmFoglalasModel>>(responseString);

        return CRMFoglalasok.Select(s => new FoglalasModel()
        {
            Id = s.orderID,
            Nev = s.kontaktNev,
            KezdDatum = s.pickupdate,
            VegeDatum = s.dropoffdate,
            RekeszId = Int32.Parse(s.locknumber),
            Email = s.kontaktEmail,
            Rendszam = s.rendszam,
            Fizetendo = Int32.Parse(s.total_price),
            Zarolando = Int32.Parse(s.deposit),
            Tipus = s.type,
            IdeiglenesFl = true
        }).ToList();
    }

    public async Task<FoglalasModel> GetFoglalasByCode(string code)
    {
        var responseString = await httpClient.GetStringAsync(options.RequestBase + "?action=pickup&qr=" + code);
        var foglalas = JsonConvert.DeserializeObject<CrmFoglalasModel>(responseString);

        return foglalas is null ? null : new FoglalasModel()
        {
            Id = foglalas.orderID,
            Nev = foglalas.kontaktNev,
            KezdDatum = foglalas.pickupdate,
            VegeDatum = foglalas.dropoffdate,
            RekeszId = Int32.Parse(foglalas.locknumber),
            Email = foglalas.kontaktEmail,
            Rendszam = foglalas.rendszam,
            Fizetendo = Int32.Parse(foglalas.total_price),
            Zarolando = Int32.Parse(foglalas.deposit),
            Tipus = foglalas.type
        };
    }

    public async void SaveEmail(int id, string email)
    {
        try
        {
            await httpClient.GetStringAsync(options.RequestBase + $"?action=addEmail&id={id}&email={email}");
        }
        catch (Exception e)
        {
            //TODO
        }
    }

    public async void SaveSignature(int id, string signature)
    {
        try
        {
            await httpClient.GetStringAsync(options.RequestBase + $"?action=addsigno&id={id}&base64={signature}");
        }
        catch (Exception e)
        {
            //TODO
        }
    }
}