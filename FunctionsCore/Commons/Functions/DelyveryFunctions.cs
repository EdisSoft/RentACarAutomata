using FunctionsCore.Commons.Entities;
using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;

namespace FunctionsCore.Commons.Functions
{
    public class DelyveryFunctions
    {
        private static ConcurrentBag<DeliveryModel> DelyveryQueue = new ConcurrentBag<DeliveryModel>();
        public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();
        private static readonly object LockObject = new object();

        public static int KezbesitesreVar()
        {
            return DelyveryQueue.Count;
        }
        public void UjCsomag(DeliveryModel csomag)
        {
            if (csomag == null)
                return;

            DelyveryQueue.Add(csomag);
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
            lock (LockObject)
            {
                while (DelyveryQueue.TryTake(out var csomag))
                {
                    if (!csomag.SendedFl)
                    {
                        switch (csomag.Type)
                        {
                            case DeliveryTypes.Email:
                                //CRM rendszernek csomag.ValueStr elküldése
                                break;
                            case DeliveryTypes.Signature:
                                //CRM rendszernek elküldeni: csomag.ValueBytes
                                break;
                            case DeliveryTypes.ScanLicenceFront:
                                ////FTP-re feltölteni: csomag.ValueBytes
                                break;
                        }
                    }
                }
            }
        }

        public static FoglalasModel UjFoglalas(FoglalasModel foglalas)
        {
            Log.Debug("Új adat érkezett! Foglalás: " + foglalas.Id);
            try
            {
                var result = FoglalasokMemory.AddOrUpdate(foglalas.Id, foglalas, (k, v) => foglalas);
                if (result != null)
                {
                    Log.Debug("Foglalas frissítése sikeres volt! FoglalasId: " + foglalas.Id);
                    return foglalas;
                }
            }
            catch (Exception e)
            {
                Log.Error("Hiba történt a foglalás frissítése közben! FoglalasId: " + foglalas.Id, e);
                throw;
            }
            return null;
        }
        public static FoglalasModel FoglalasTorles(FoglalasModel foglalas)
        {
            Log.Debug("Foglalás törlés: " + foglalas.Id);
            try
            {
                FoglalasModel resultModel;
                var foglalasRemoved = FoglalasokMemory.TryRemove(foglalas.Id, out resultModel);
                if (foglalasRemoved)
                {
                    Log.Debug("Foglalás törlése sikeres volt. FoglalasId: " + foglalas.Id);
                    return foglalas;
                }
                Log.Info("Foglalás törlése sikertelen volt. FoglalasId: " + foglalas.Id);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("Hiba történt a foglalás törlése közben! FoglalasId: " + foglalas.Id, e);
                throw;
            }
        }

    }
}