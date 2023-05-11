using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionsCore.Commons.Functions;

public class BookingFunctions : IBookingFunctions
{
    private static ConcurrentBag<DeliveryModel> deliveryQueue = new ConcurrentBag<DeliveryModel>();
    private static readonly object lockObject = new object();
    private IHTTPRequestService requestService;

    public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();

    public BookingFunctions(IHTTPRequestService requestService)
    {
        this.requestService = requestService;
    }

    public static int KezbesitesreVar()
    {
        return deliveryQueue.Count;
    }

    public void UjCsomag(DeliveryModel csomag)
    {
        if (csomag == null)
        {
            return;
        }

        deliveryQueue.Add(csomag);
    }

    public void KuldesAsync()
    {
        var svc = BaseAppContext.Instance.CurrentHttpContext.RequestServices;
        var queue = (IBackgroundTaskQueue)svc.GetService(typeof(IBackgroundTaskQueue));
        queue.QueueBackgroundWorkItem(async token =>
        {
            var task = Task.Run(() =>
            {
                Kuldes();
            });
            await task;
        });
    }

    public void Kuldes()
    {
        lock (lockObject)
        {
            while (deliveryQueue.TryTake(out var csomag))
            {
                if (!csomag.SendedFl)
                {
                    switch (csomag.Type)
                    {
                        case DeliveryTypes.Email:
                            requestService.SaveEmail(csomag.OrderId, csomag.ValueStr);
                            break;
                        case DeliveryTypes.Signature:
                            requestService.SaveSignature(csomag.OrderId, csomag.ValueStr);
                            break;
                        case DeliveryTypes.ScanLicenceFront:
                            ////FTP-re feltölteni: csomag.ValueBytes
                            break;
                        case DeliveryTypes.KeyTaken:
                            FoglalasTorles(csomag.ValueInt);
                            break;
                    }
                }
            }
        }
    }

    public static void SikeresFoglalas(int id, string nyelv)
    {
        try
        {
            if (FoglalasokMemory.TryGetValue(id, out var foglalas))
            {
                foglalas.Nyelv = nyelv == Nyelvek.Magyar.ToString() ? Nyelvek.Magyar : Nyelvek.English;
                foglalas.IdeiglenesFl = false;
                Log.Debug("Foglalas megerősítése sikeres volt! FoglalasId: " + foglalas.Id);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás frissítése közben! FoglalasId: " + id, e);
        }
    }

    public static FoglalasModel FindFoglalasById(int id)
    {
        Log.Debug($"Foglalás keresése. Foglalás: {id}");
        try
        {
            FoglalasModel resultModel;
            var foglalasFound = FoglalasokMemory.TryGetValue(id, out resultModel);
            if (foglalasFound)
            {
                Log.Debug($"Foglalas frissítése sikeres volt! FoglalasId: {resultModel.Id}");
                return resultModel;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Hiba történt a foglalás frissítése közben! FoglalasId: {id}", e);
            throw;
        }
        return null;
    }

    public static void UpdateFoglalas(int foglalasId, string nyelv)
    {
        try
        {
            if (FoglalasokMemory.TryGetValue(foglalasId, out var foglalasMemory))
            {
                switch (nyelv)
                {
                    case "en":
                        foglalasMemory.Nyelv = Nyelvek.English;
                        break;
                    case "hu":
                        foglalasMemory.Nyelv = Nyelvek.Magyar;
                        break;
                }

                foglalasMemory.IdeiglenesFl = false;
                foglalasMemory.UtolsoVarazsloLepes = 3;

                var itemKeysToRemove = FoglalasokMemory.Where(w => w.Value.IdeiglenesFl == true).Select(s => s.Key).ToList();

                foreach (var itemKey in itemKeysToRemove)
                {
                    BookingFunctions.FoglalasTorles(itemKey);
                }

                Log.Debug("Foglalas frissítése sikeres volt! FoglalasId: " + foglalasId);
            }
            else
            {
                throw new WarningException("Nincs foglalás!", WarningExceptionLevel.Warning);
            }

        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás frissítése közben! FoglalasId: " + foglalasId, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
    }

    public static void UjFoglalas(FoglalasModel foglalas)
    {
        try
        {
            if (FoglalasokMemory.TryGetValue(foglalas.Id, out var foglalasMemory))
            {
                foglalasMemory = foglalas;
                Log.Debug("Foglalas frissítése sikeres volt! FoglalasId: " + foglalas.Id);
            }
            else
            {
                if (!FoglalasokMemory.TryAdd(foglalas.Id, foglalas))
                {
                    Log.Error("Foglalás hozzáadása sikertelen! FoglalasId: " + foglalas.Id);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás hozzáadása közben! FoglalasId: " + foglalas.Id, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
    }

    public static void UjFoglalas(List<FoglalasModel> foglalasok)
    {
        foreach (var foglalas in foglalasok)
        {
            UjFoglalas(foglalas);
        }
    }

    public static void FoglalasTorles(int id)
    {
        try
        {
            if (FoglalasokMemory.Remove(id, out var foglalas))
            {
                Log.Debug("Foglalás törlése sikeres volt. FoglalasId: " + id);
            }
            else
            {
                Log.Info("Foglalás törlése sikertelen volt. FoglalasId: " + id);
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás törlése közben! FoglalasId: " + id, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
    }
}