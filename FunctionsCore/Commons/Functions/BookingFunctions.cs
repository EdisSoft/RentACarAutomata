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
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Authentication;

namespace FunctionsCore.Commons.Functions;

public class BookingFunctions : IBookingFunctions
{
    private static ConcurrentBag<DeliveryModel> deliveryQueue = new ConcurrentBag<DeliveryModel>();
    private static readonly object lockObject = new object();
    private IHttpRequestService requestService;
    private FTPConnectionOptions FTPConnectionOptions;
    private FileNameOptions fileNameOptions;

    public static ConcurrentDictionary<int, FoglalasModel> FoglalasokMemory = new ConcurrentDictionary<int, FoglalasModel>();

    public BookingFunctions(IHttpRequestService requestService, IConfiguration configuration)
    {
        FTPConnectionOptions = configuration.GetSection(nameof(FTPConnectionOptions)).Get<FTPConnectionOptions>();
        fileNameOptions = configuration.GetSection(nameof(fileNameOptions)).Get<FileNameOptions>();

        this.requestService = requestService;
    }

    public static int KezbesitesreVar()
    {
        return deliveryQueue.Count;
    }

    public static bool VanAktivUgyfel()
    {
        bool bVanUgyfel = false;

        var aktivFoglalasok = FoglalasokMemory.Where(w => w.Value.AktivUgyfelFl);
        bVanUgyfel = (aktivFoglalasok.Count() > 0);
        return bVanUgyfel;
    }

    public void UjCsomag(DeliveryModel csomag)
    {
        if (csomag == null)
        {
            return;
        }

        Log.Info($"BookingFunctions.UjCsomag: {csomag.Id}");
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

    private void KeszFoglalasokTorlese()
    {
        var torolhetoFoglalasok = FoglalasokMemory.Where(w => w.Value.TorolhetoFl);

        foreach (var foglalas in torolhetoFoglalasok)
        {
            var foglalasId = foglalas.Key;
            if (!deliveryQueue.Any(w => w.OrderId == foglalasId))
            {
                FoglalasTorles(foglalasId);
            }
        }
    }

    public async void Kuldes()
    {
        KeszFoglalasokTorlese();
        while (deliveryQueue.TryTake(out var csomag))
        {
#if DEBUG1
            continue;
#endif

            switch (csomag.Type)
            {
                case DeliveryTypes.Email:
                    if (!await requestService.SaveEmail(csomag.OrderId, csomag.ValueStr))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SaveTextInLocalFolder(csomag.OrderId.ToString(), csomag.ValueStr, DeliveryTypes.Email.ToString());
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }
                    break;

                case DeliveryTypes.Signature:
                    if (!await requestService.SaveSignature(csomag.OrderId, csomag.ValueStr))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SaveTextInLocalFolder(csomag.OrderId.ToString(), csomag.ValueStr, DeliveryTypes.Signature.ToString());
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }
                    break;

                case DeliveryTypes.ScanLicenceFront:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.LicenseFront))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.LicenseFront);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }
                    break;

                case DeliveryTypes.ScanLicenceBack:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.LicenseBack))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.LicenseBack);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }

                    break;

                case DeliveryTypes.ScanIdCardFrontOrPassport:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.IdCardFrontOrPassport))
                    {
                        csomag.NumberOfSending++;

                        if (csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.IdCardFrontOrPassport);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }

                    break;

                case DeliveryTypes.ScanIdCardBack:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.IdCardBack))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.IdCardBack);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }

                    break;

                case DeliveryTypes.ScanCreditCardFront:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.CreditCardFront))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.CreditCardFront);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }

                    break;

                case DeliveryTypes.ScanCreditCardBack:
                    if (!UploadImage(csomag.OrderId, csomag.ValueBytes, fileNameOptions.CreditCardBack))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SavePictureInLocalFolder(csomag.OrderId.ToString(), csomag.ValueBytes, fileNameOptions.CreditCardBack);
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }

                    break;

                case DeliveryTypes.Deposit:
                    if (!await requestService.SendDeposit(csomag.OrderId, csomag.ValueNyelv.ToString(), csomag.ValueInt, csomag.ValueStr))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SaveTextInLocalFolder(csomag.OrderId.ToString(), csomag.ValueInt.ToString() + " " + csomag.ValueStr, DeliveryTypes.Deposit.ToString());
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }
                    break;

                case DeliveryTypes.Payment:
                    if (!await requestService.SendPayment(csomag.OrderId, csomag.ValueNyelv.ToString(), csomag.ValueInt, csomag.ValueStr))
                    {
                        if (++csomag.NumberOfSending > 5)
                        {
                            SaveTextInLocalFolder(csomag.OrderId.ToString(), csomag.ValueInt.ToString() + " " + csomag.ValueStr, DeliveryTypes.Payment.ToString());
                        }
                        else
                        {
                            UjCsomag(csomag);
                        }
                    }
                    else
                    {
                        UpdateFoglalasTorolheto(csomag.OrderId);
                    }
                    break;
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
            Log.Error($"Hiba történt a foglalás frissítése közben (FindFoglalasById)! FoglalasId: {id}", e);
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
        UjFoglalasVagyModositas(foglalas);
    }

    public static void UpdateFoglalasNyelv(int foglalasId, string nyelv)
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
                foglalasMemory.UtolsoVarazsloLepes = 4; //3+1
                UjFoglalasVagyModositas(foglalasMemory);

                var itemKeysToRemove = FoglalasokMemory.Where(w => w.Value.IdeiglenesFl == true).Select(s => s.Key).ToList();

                foreach (var itemKey in itemKeysToRemove)
                {
                    FoglalasTorles(itemKey);
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
            Log.Error("Hiba történt a foglalás frissítése közben (UpdateFoglalasNyelv)! FoglalasId: " + foglalasId, e);
            throw new WarningException("Hiba történt!", WarningExceptionLevel.Warning);
        }
    }

    public static void UpdateFoglalasTorolheto(int foglalasId)
    {
        try
        {
            if (FoglalasokMemory.TryGetValue(foglalasId, out var foglalasMemory))
            {
                foglalasMemory.TorolhetoFl = true;
                UjFoglalasVagyModositas(foglalasMemory);
                Log.Info("Foglalas TorolhetFl=true: " + foglalasId);
            }
            else
            {
                throw new WarningException("Nincs foglalás (UpdateFoglalasTorolheto)!", WarningExceptionLevel.Warning);
            }

        }
        catch (Exception e)
        {
            Log.Error("Hiba történt a foglalás frissítése közben (UpdateFoglalasTorolheto)! FoglalasId: " + foglalasId, e);
        }
    }

    public static void UjFoglalas(List<FoglalasModel> foglalasok)
    {
        foreach (var foglalas in foglalasok)
        {
            UjFoglalasVagyModositas(foglalas);
        }
    }

    public static FoglalasModel UjFoglalasVagyModositas(FoglalasModel foglalas)
    {
        Log.Debug("Új adat érkezett! Foglalás: " + foglalas.Id);

        int folyamatbanLevoVarazsloLepes = FindFoglalasById(foglalas.Id)?.UtolsoVarazsloLepes ?? 0;

        try
        {
            foglalas.ZarolvaFl = foglalas.ZarolvaFl || foglalas.Zarolando == 0; // Zárolás kész
            foglalas.FizetveFl = foglalas.FizetveFl || foglalas.Fizetendo == 0; // Fizetés kész
            foglalas.UtolsoModositas = DateTime.Now;
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
        Log.Info("Foglalás törlés: " + foglalasId);
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

    private bool UploadImage(int id, byte[] picture, string pictureName)
    {
        Log.Info($"BookingFunctions.UploadImage({id},{pictureName})");

        var path = $"{FTPConnectionOptions.Address}/{id}/";

        if (DoesFtpDirectoryExist(path) == false)
        {
            Log.Error("Hiba történt FTP mappa létrehozása közben! FoglalasId: " + id);
            return false;
        }

        FtpWebRequest req = null;
        WebResponse response = null;
        try
        {
            req = (FtpWebRequest)WebRequest.Create(path + "\\" + pictureName); //$"{path}/{pictureName}"

            req.Credentials = new NetworkCredential(FTPConnectionOptions.UserName, FTPConnectionOptions.Password);
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
            return false;
        }
        finally
        {
            if (response != null)
            {
                response.Close();
            }
        }
        return true;
    }

    private bool UploadImageWithHttpClient(int id, byte[] picture, string pictureName)
    {
        Log.Info($"BookingFunctions.UploadImage({id},{pictureName})");

        var path = $"{FTPConnectionOptions.Address}/{id}/";

        if (DoesFtpDirectoryExist(path) == false)
        {
            Log.Error("Hiba történt FTP mappa létrehozása közben! FoglalasId: " + id);
            return false;
        }

        HttpClient client = null;
        ByteArrayContent content = null;
        try
        {
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.ConnectionClose = false;
                client.DefaultRequestHeaders.Connection.Add("keep-alive");
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
                client.DefaultRequestHeaders.ExpectContinue = false;
                using (content = new ByteArrayContent(picture))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    var handler = new HttpClientHandler
                    {
                        Credentials = new NetworkCredential(FTPConnectionOptions.UserName, FTPConnectionOptions.Password),
                        SslProtocols = SslProtocols.Tls12
                    };
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    using (var clientWithCredentials = new HttpClient(handler))
                    {
                        var response = clientWithCredentials.PutAsync($"{path}/{pictureName}", content);
                        return response.Status != TaskStatus.Faulted && response.Status != TaskStatus.Canceled;
                    }
                }
            }
        }
        catch (WebException ex)
        {
            Log.Error("Hiba történt fájl feltöltése közben! FoglalasId: " + id, ex);
            return false;
        }
        finally
        {
            if (content != null)
                content.Dispose();
            if (client != null)
                content.Dispose();
        }
    }

    public async void SavePictureInLocalFolder(string id, byte[] picture, string pictureName)
    {
        var path = fileNameOptions.TempPath + id;
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            await File.WriteAllBytesAsync(Path.Combine(path, pictureName), picture);
        }
        catch
        {
            Log.Warning("Hiba temp mappába mentés közben");
        }
    }

    public async void SaveTextInLocalFolder(string id, string text, string type)
    {
        var path = fileNameOptions.TempPath + id;
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fullPath = path + "\\" + fileNameOptions.Backup;

            string[] lines = { type + ": " + text, "" };

            await File.AppendAllLinesAsync(fullPath, lines);
        }
        catch
        {
            Log.Warning("Hiba temp mappába mentés közben");
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
            request.Credentials = new NetworkCredential(FTPConnectionOptions.UserName, FTPConnectionOptions.Password);
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

    public int? SetTempValues(int foglalasId, List<int> rekeszIds)
    {
        FoglalasKucsLeadasModel.Id = foglalasId;
        FoglalasKucsLeadasModel.RekeszIds = rekeszIds;
        FoglalasKucsLeadasModel.RekeszId = rekeszIds.FirstOrDefault();
        return FoglalasKucsLeadasModel.RekeszId;
    }

    public int? GetRekeszId(int foglalasId)
    {
        if (FoglalasKucsLeadasModel.Id == foglalasId)
            return FoglalasKucsLeadasModel.RekeszId;
        return null;
    }
}