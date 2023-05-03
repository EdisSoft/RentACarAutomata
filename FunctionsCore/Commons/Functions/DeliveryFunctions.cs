using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;
using System.Collections.Generic;

namespace FunctionsCore.Commons.Functions;

public class DeliveryFunctions : IDeliveryFunctions
{
    private static ConcurrentBag<DeliveryModel> deliveryQueue = new ConcurrentBag<DeliveryModel>();
    private static readonly object lockObject = new object();
    private IHTTPRequestService requestService;

    public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();

    public DeliveryFunctions(IHTTPRequestService requestService)
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
                foglalas.Nyelv = nyelv == Nyelvek.Magyar.ToString() ? Nyelvek.Magyar : Nyelvek.Angol;
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
                if(!FoglalasokMemory.TryAdd(foglalas.Id, foglalas)) 
                {
                    Log.Error("Foglalás hozzáadása sikertelen! FoglalasId: " + foglalas.Id);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás hozzáadása közben! FoglalasId: " + foglalas.Id, e);
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
            throw;
        }
    }
}