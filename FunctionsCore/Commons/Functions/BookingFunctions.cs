using FunctionsCore.Contexts;
using FunctionsCore.Services;
using FunctionsCore.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FunctionsCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FunctionsCore.Commons.Functions;

public class BookingFunctions : IBookingFunctions
{
    private static ConcurrentBag<DeliveryModel> deliveryQueue = new ConcurrentBag<DeliveryModel>();
    private static readonly object lockObject = new object();
    private IHttpRequestService requestService;
    private FTPConnectionOptions options;
    private static int tempFoglalasId { get; set; }
    private static List<int> tempRekeszIds { get; set; }
    private static int tempRekeszId { get; set; }


    public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();

    public BookingFunctions(IHttpRequestService requestService, IConfiguration configuration)
    {
        options = configuration.GetSection(nameof(FTPConnectionOptions)).Get<FTPConnectionOptions>();

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
                            try
                            {
                                requestService.SaveEmail(csomag.OrderId, csomag.ValueStr);
                            }
                            catch (Exception e)
                            {                                
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt email küldése közben!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.Signature:
                            try
                            {
                                requestService.SaveSignature(csomag.OrderId, csomag.ValueStr);
                            }
                            catch (Exception e)
                            {                                
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt aláírás mentése közben!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanLicenceFront:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "LicenseFront.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanLicenceBack:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "LicenseBack.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanIdCardFrontOrPassport:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "IdCardFrontOrPassport.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanIdCardBack:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "IdCardBack.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanCreditCardFront:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "CreditCardFront.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.ScanCreditCardBack:
                            try
                            {
                                UploadImage(csomag.Id, csomag.ValueBytes, "CreditCardBack.jpg");
                            }
                            catch
                            {
                                UjCsomag(csomag);
                                throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.Deposit:
                            try
                            {
                                requestService.SendDeposit(csomag.OrderId, csomag.ValueNyelv.ToString(), csomag.ValueInt, csomag.ValueStr);
                            }
                            catch (Exception e)
                            {
                                UjCsomag(csomag);
                                throw new WarningException(e.Message, WarningExceptionLevel.Warning);
                            }
                            break;
                        case DeliveryTypes.Payment:
                            var paymentSent = false;
                            try
                            {
                                requestService.SendPayment(csomag.OrderId, csomag.ValueNyelv.ToString(), csomag.ValueInt, csomag.ValueStr);
                                paymentSent = true;
                                FoglalasTorles(csomag.ValueInt);
                            }
                            catch (Exception e)
                            {
                                if (!paymentSent)
                                {
                                    UjCsomag(csomag);
                                }
                                throw new WarningException(e.Message, WarningExceptionLevel.Warning);
                            }
                            break;
                        //case DeliveryTypes.KeyTaken:
                        //    FoglalasTorles(csomag.ValueInt);
                        //    break;
                    }
                }
            }
        }
    }


    public static FoglalasModel FindFoglalasById(int id)
    {
        Log.Debug($"Foglalás keresése. Foglalás: {id}");
        try
        {
            var foglalasFound = FoglalasokMemory.TryGetValue(id, out var resultModel);
            if (foglalasFound)
            {
                Log.Debug($"Foglalas kikeresése sikeres volt! FoglalasId: {resultModel.Id}");
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

    public static void UpdateUtolsoVarazsloLepes(int id, int varazsloLepes)
    {
        var foglalas = FindFoglalasById(id);
        if (foglalas is null)
        {
            Log.Error($"Hiba történt a foglalás frissítése közben! FoglalasId: {id}");
            throw new WarningException("Nincs meg a foglalás!", WarningExceptionLevel.Warning);
        }
        foglalas.UtolsoVarazsloLepes = varazsloLepes;
        UjFoglalas(foglalas);
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
                        foglalasMemory.Nyelv = Nyelvek.en;
                        break;
                    case "hu":
                        foglalasMemory.Nyelv = Nyelvek.hu;
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

    public static void UjFoglalas(List<FoglalasModel> foglalasok)
    {
        foreach (var foglalas in foglalasok)
        {
            UjFoglalas(foglalas);
        }
    }

    public static FoglalasModel UjFoglalas(FoglalasModel foglalas)
    {
        Log.Debug("Új adat érkezett! Foglalás: " + foglalas.Id);

        int folyamatbanLevoVarazsloLepes = FindFoglalasById(foglalas.Id)?.UtolsoVarazsloLepes ?? 0;

        try
        {
            foglalas.ZarolvaFl = foglalas.Zarolando == 0; // Zárolás kész
            foglalas.FizetveFl = foglalas.Fizetendo == 0; // Fizetés kész
            var result = FoglalasokMemory.AddOrUpdate(foglalas.Id, foglalas, (k, v) => foglalas);
            if (result != null)
            {
                if (folyamatbanLevoVarazsloLepes == 0)
                    Log.Debug("Új foglalas felvétele sikeres volt! FoglalasId: " + foglalas.Id);
                else
                {
                    Log.Debug("Foglalas frissítése sikeres volt! FoglalasId: " + foglalas.Id);
                    result.UtolsoVarazsloLepes = folyamatbanLevoVarazsloLepes;
                }
                return foglalas;
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás frissítése közben! FoglalasId: " + foglalas.Id, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
        return null;
    }

    public static void FoglalasTorles(int foglalasId)
    {
        Log.Debug("Foglalás törlés: " + foglalasId);
        try
        {
            FoglalasModel resultModel;
            var foglalasRemoved = FoglalasokMemory.TryRemove(foglalasId, out resultModel);
            if (foglalasRemoved)
            {
                Log.Debug("Foglalás törlése sikeres volt. FoglalasId: " + foglalasId);
            }
            else
            {
                Log.Info("Foglalás törlése sikertelen volt. FoglalasId: " + foglalasId);
            }
        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás törlése közben! FoglalasId: " + foglalasId, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
    }

    private void UploadImage(string id, byte[] picture, string pictureName)
    {
        var path = $"{options.Address}/{id}/";

        if (DoesFtpDirectoryExist(path) == false)
        {
            Log.Error("Hiba történt FTP mappa létrehozása közben! FoglalasId: " + id);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }

        FtpWebRequest req = null;
        WebResponse response = null;
        try
        {
            req = (FtpWebRequest)WebRequest.Create(path + "\\" + pictureName);

            req.Credentials = new NetworkCredential(options.UserName, options.Password);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = picture.Length;
            req.UseBinary = true;
            req.KeepAlive = false;
            req.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (Stream sw = req.GetRequestStream())
            {
                sw.Write(picture, 0, picture.Length);
            }

            response = req.GetResponse();
        }
        catch (WebException ex)
        {
            Log.Error("Hiba történt fájl feltöltése közben! FoglalasId: " + id, ex);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
    }

    private bool DoesFtpDirectoryExist(string path)
    {
        var result = true;
        FtpWebResponse response = null;
        Stream ftpStream = null;
        try
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(options.UserName, options.Password);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            response = (FtpWebResponse)request.GetResponse();
            ftpStream = response.GetResponseStream();
        }
        catch (WebException ex)
        {
            FtpWebResponse exResponse = (FtpWebResponse)ex.Response;

            result = exResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable;
            exResponse.Close();
        }
        finally
        {
            if (response is not null)
            {
                response.Close();
            }
            if (ftpStream is not null)
            {
                ftpStream.Close();
            }
        }

        return result;
    }

    public int SetTempValues(int foglalasId, List<int> rekeszIds)
    {
        tempFoglalasId = foglalasId;
        tempRekeszIds = rekeszIds;
        tempRekeszId = rekeszIds.FirstOrDefault();
        return tempRekeszId;
    }

    public int GetRekeszId(int foglalasId)
    {
        return tempRekeszId;
    }
}