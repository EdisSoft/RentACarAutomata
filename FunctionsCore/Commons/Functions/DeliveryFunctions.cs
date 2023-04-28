using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;

namespace FunctionsCore.Commons.Functions
{
    public class DeliveryFunctions : IDeliveryFunctions
    {
        private static ConcurrentBag<DeliveryModel> DeliveryQueue = new ConcurrentBag<DeliveryModel>();
        private static readonly object LockObject = new object();
        private IHTTPRequestService requestService;

        public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();

        public DeliveryFunctions(IHTTPRequestService requestService)
        {
            this.requestService = requestService;
        }

        public static int KezbesitesreVar()
        {
            return DeliveryQueue.Count;
        }

        public void UjCsomag(DeliveryModel csomag)
        {
            if (csomag == null)
            {
                return;
            }

            DeliveryQueue.Add(csomag);
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
                while (DeliveryQueue.TryTake(out var csomag))
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
                var foglalasRemoved = FoglalasokMemory.TryRemove(foglalas.Id, out var resultModel);
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