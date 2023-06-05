using FunctionsCore.Models;
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
        Log.Info($"HttpRequestService.GetFoglalasokByNev({nev})");

        var responseString = await httpClient.GetStringAsync(options.RequestBase + "?action=pickup&mainparam=" + nev);
        var CRMFoglalasok = JsonConvert.DeserializeObject<List<CrmFoglalasModel>>(responseString);

        //mock
        //var res = new List<FoglalasModel>();

        //res.Add(new FoglalasModel()
        //{
        //    Id = 1,
        //    Nev = "Teszt",
        //    KezdDatum = DateTime.Now,
        //    VegeDatum = DateTime.Now.AddDays(7),
        //    RekeszId = 3,
        //    Rendszam = "APA565",
        //    Email = "gabor@mail.hu",
        //    Fizetendo = 500,
        //    Zarolando = 200000,
        //    Tipus = "user",
        //    IdeiglenesFl = false
        //});
        //return res;

        return CRMFoglalasok.Select(s => new FoglalasModel()
        {
            Id = s.orderID,
            Nev = s.kontaktNev,
            KezdDatum = s.pickupdate,
            VegeDatum = s.dropoffdate,
            RekeszId = Int32.Parse(s.locknumber),
            Rendszam = s.rendszam,
            Email = s.kontaktEmail,
            Fizetendo = Int32.Parse(s.total_price),
            Zarolando = Int32.Parse(s.deposit),
            Tipus = s.type,
            IdeiglenesFl = true
        }).ToList();
    }

    public async Task<FoglalasModel> GetFoglalasByCode(string code)
    {
        Log.Info($"HttpRequestService.GetFoglalasByCode({code})");
        var responseString = await httpClient.GetStringAsync(options.RequestBase + "?action=pickup&qr=" + code);
        var foglalas = JsonConvert.DeserializeObject<CrmFoglalasModel>(responseString);

        Log.Info($"HttpRequestService.GetFoglalasByCode foglalasId: {foglalas.orderID}");
        return foglalas is null ? null : new FoglalasModel()
        {
            Id = foglalas.orderID,
            Nev = foglalas.kontaktNev,
            KezdDatum = foglalas.pickupdate,
            VegeDatum = foglalas.dropoffdate,
            RekeszId = Int32.Parse(foglalas.locknumber),
            Rendszam = foglalas.rendszam,
            Email = foglalas.kontaktEmail,
            Fizetendo = Int32.Parse(foglalas.total_price),
            Zarolando = Int32.Parse(foglalas.deposit),
            Tipus = foglalas.type
        };
    }

    public async void SaveEmail(int id, string email)
    {
        try
        {
            Log.Info($"HttpRequestService.SaveEmail({id})");
            await httpClient.GetStringAsync(options.RequestBase + $"?action=addEmail&id={id}&email={email}");
        }
        catch (Exception e)
        {
            Log.Error($"Hiba email mentése közben! Foglalás: {id}", e);
            throw new WarningException("Hiba email mentése közben!", WarningExceptionLevel.Warning);
        }
    }

    public async void SaveSignature(int id, string signature)
    {
        try
        {
            Log.Info($"HttpRequestService.SaveSignature({id})");
            await httpClient.GetStringAsync(options.RequestBase + $"?action=addsigno&id={id}&base64={signature}");
        }
        catch (Exception e)
        {
            Log.Error($"Hiba aláírás mentése közben! Foglalás: {id}", e);
            throw new WarningException("Hiba aláírás mentése közben!", WarningExceptionLevel.Warning);
        }
    }

    public async void SendDeposit(int id, string language, int deposittrid, string slip)
    {
        try
        {
            Log.Info($"HttpRequestService.SendDeposit({id},{language},{deposittrid})");
            await httpClient.GetStringAsync(options.RequestBase + $"?action=deposit&id={id}&lang={language}&deposittrid={deposittrid}&slip={slip}");
        }
        catch (Exception e)
        {
            Log.Error($"Hiba deposit CRM küldése közben! Foglalás: {id}", e);
            throw new WarningException("Hiba deposit CRM küldése közben!", WarningExceptionLevel.Warning);
        }
    }

    public async void SendPayment(int id, string language, int paymenttrid, string slip)
    {
        try
        {
            Log.Info($"HttpRequestService.SendPayment({id},{language},{paymenttrid})");
            await httpClient.GetStringAsync(options.RequestBase + $"?action=payment&id={id}&lang={language}&paymenttrid={paymenttrid}&slip={slip}");
        }
        catch (Exception e)
        {
            Log.Error($"Hiba fizetés CRM küldése közben! Foglalás: {id}", e);
            throw new WarningException("Hiba fizetés CRM küldése közben!", WarningExceptionLevel.Warning);
        }
    }

    public async Task<AutoLeadasModel> KocsiLeadas(string rendszam)
    {
        try
        {
            Log.Info($"HttpRequestService.KocsiLeadas({rendszam})");
            var responseString = await httpClient.GetStringAsync(options.RequestBase + $"?action=dropoff&rendszam={rendszam}");

            var entity = JsonConvert.DeserializeObject<CrmAutoLeadasModel>(responseString);
            if (entity == null)
                return null;

            return new AutoLeadasModel()
            {
                Id = entity.Id,
                Rendszam = entity.Rendszam,
                RfId = entity.Keyid,
                RekeszId = entity.LockNumber
            };
        }
        catch (Exception e)
        {
            Log.Error($"Hiba auto leadása közben! Rendszám: {rendszam}", e);
            throw new WarningException($"Hiba auto leadása közben! Rendszám: {rendszam}", WarningExceptionLevel.Warning);
        }
    }

    public async Task KulcsLeadas(int id, int rekeszId, bool taxiFl)
    {
        try
        {
            Log.Info($"HttpRequestService.KulcsLeadas({id})");
            var taxiInt = taxiFl ? 1 : 0;
            await httpClient.GetStringAsync(options.RequestBase + $"?action=finished&id={id}&locknumber={rekeszId}&taxi={taxiInt}");
        }
        catch (Exception e)
        {
            Log.Error($"Hiba kulcs leadása közben! Foglalás: {id}", e);
            throw new WarningException($"Hiba kulcs leadása közben! Foglalás: {id}", WarningExceptionLevel.Warning);
        }
    }
}
