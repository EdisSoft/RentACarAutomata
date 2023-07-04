using Automata.Functions;
using FunctionsCore;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Enums;
using FunctionsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Automata.Controllers;

public class FoglalasController : BaseController
{
    private ICrmFunctions CrmFunctions { get; set; }
    private IBookingFunctions BookingFunctionsInst { get; set; }
    private IIdScannerFunctions IdScannerFunctionsInst { get; set; }
    private IKerongLockFunctions KerongLockFunctions { get; set; }

    public FoglalasController(ICrmFunctions crmFunctions, IBookingFunctions bookingFunctions, IIdScannerFunctions idScannerFunctions, IKerongLockFunctions kerongLockFunctions)
    {
        CrmFunctions = crmFunctions;
        BookingFunctionsInst = bookingFunctions;
        IdScannerFunctionsInst = idScannerFunctions;
        KerongLockFunctions = kerongLockFunctions;
    }

    [HttpPost]
    public async Task<JsonResult> GetFoglalasok(string nev)
    {
        nev = nev.Replace("  ", " ").Trim();

        if (nev.Length < 5)
            throw new WarningException("Name too short");

        var result = await CrmFunctions.GetFoglalasokByNev(nev);
        BookingFunctions.UjFoglalas(result);

        return Json(result);
    }

    public async Task<JsonResult> ReadQr()
    {
        var booking = await CrmFunctions.GetFoglalasByQrCode();
        if (booking != null && booking.Tipus != "admin")
        {
            if (booking.RekeszId == 0)
                throw new WarningException("Sorry, the car key is not in the locker yet");

            BookingFunctions.UjFoglalasVagyModositas(booking);
        }

        return Json(booking);
    }

    [HttpPost]
    public JsonResult SaveEmail(int id, string email)
    {
        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueStr = email,
            Type = DeliveryTypes.Email
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult SaveAlairas([FromBody] AlairasModel model)
    {
        var booking = BookingFunctions.FindFoglalasById(model.Id);
        if (booking.RekeszId == 0)
            throw new WarningException("Sorry, the car key is not in the locker yet. Go to the home screen");

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = model.Id,
            ValueStr = model.Pic,
            Type = DeliveryTypes.Signature
        });

        var nyelv = Request.Headers["Accept-Language"];
        BookingFunctions.UpdateFoglalasNyelv(model.Id, nyelv);

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceFront(int id)
    {
        Log.Info("ScanLicenseFront started");
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentTypeValidation") != 0)
        {
            if (model.Tipus != DocumentTypes.DrivingLicenceFront && model.Tipus != DocumentTypes.DrivingLicenceBack)
            {
                Log.Warning($"Foglalas/ScanLicenceFront rossz típus: {model.Tipus}");

                if (model.Tipus != DocumentTypes.IdCardBack) // Nemzetközi jogsinál ennek ismerte fel, így engedjük
                    throw new WarningException("Wrong document type");
            }
        }

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentAuthenticityCheck") != 0)
        {
            if (model.EredetisegValoszinusege <= 0)
            {
                throw new WarningException("Document maybe invalid");
            }
        }

        if (!IdScannerFunctions.NevEgyezikReszbenFl("ScanLicenceFront", id, model.Nev, 3))
        {
            throw new WarningException("Wrong name on the document");
        }

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanLicenceFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 6); // 5+1

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanLicenceBack(int id)
    {
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentTypeValidation") != 0)
        {
            if (model.Tipus != DocumentTypes.DrivingLicenceFront && model.Tipus != DocumentTypes.DrivingLicenceBack)
            {
                Log.Info($"Foglalas/ScanLicenceBack rossz típus: {model.Tipus}");
                //throw new WarningException("Wrong document type");
            }
        }

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentAuthenticityCheck") != 0)
        {
            if (model.EredetisegValoszinusege <= 0)
            {
                throw new WarningException("Document maybe invalid");
            }
        }

        var booking = BookingFunctions.FindFoglalasById(id);
        Log.Info($"IdScannerFunctions.NevEgyezikReszbenFl: {IdScannerFunctions.NevEgyezikReszbenFl("ScanLicenceBack", id, model.Nev, 3)}, foglalás: {booking.Nev}, ocr: {model.Nev}");

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanLicenceBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanIdCardFrontOrPassport(int id)
    {
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentAuthenticityCheck") != 0)
        {
            if (model.EredetisegValoszinusege <= 0)
            {
                throw new WarningException("Document maybe invalid");
            }
        }

        if (model.Tipus == DocumentTypes.IdCardFront || model.Tipus == DocumentTypes.Passport)
        {
            if (!IdScannerFunctions.NevEgyezikReszbenFl("ScanIdCardFrontOrPassport", id, model.Nev, 3))
            {
                throw new WarningException("Wrong name on the document");
            }

            BookingFunctionsInst.UjCsomag(new DeliveryModel()
            {
                OrderId = id,
                ValueBytes = model.Kep,
                Type = DeliveryTypes.ScanIdCardFrontOrPassport
            });

            BookingFunctions.UpdateUtolsoVarazsloLepes(id, 7); //6+1

            bool passportFl = model.Tipus == DocumentTypes.Passport;
            return Json(new ResultModel() { Id = passportFl.GetHashCode(), Text = model.Tipus.ToString() }); //Az útlevél egy oldalas, így a UI továbblép
        }

        Log.Warning($"Foglalas/ScanIdCardFrontOrPassport rossz típus: {model.Tipus}");
        throw new WarningException("Please scan your passport or the front page of identity card.<br/>Your document may be expired or not valid.");
    }

    [HttpPost]
    public JsonResult ScanIdCardBack(int id)
    {
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentTypeValidation") != 0)
        {
            if (model.Tipus != DocumentTypes.IdCardFront && model.Tipus != DocumentTypes.IdCardBack)
            {
                Log.Warning($"Foglalas/ScanIdCardBack rossz típus: {model.Tipus}");
                throw new WarningException("Wrong document type");
            }
        }

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentAuthenticityCheck") != 0)
        {
            if (model.EredetisegValoszinusege <= 0)
            {
                throw new WarningException("Document maybe invalid");
            }
        }

        var booking = BookingFunctions.FindFoglalasById(id);
        Log.Info($"IdScannerFunctions.NevEgyezikReszbenFl: {IdScannerFunctions.NevEgyezikReszbenFl("ScanIdCardBack", id, model.Nev, 3)}, foglalás: {booking.Nev}, ocr: {model.Nev}");

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanIdCardBack
        });

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardFront(int id)
    {
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentTypeValidation") != 0)
        {
            if (model.Tipus == DocumentTypes.Passport || model.Tipus == DocumentTypes.DrivingLicenceFront || model.Tipus == DocumentTypes.DrivingLicenceBack ||
                model.Tipus == DocumentTypes.IdCardFront || model.Tipus == DocumentTypes.IdCardBack)
            {
                Log.Info($"Foglalas/ScanCreditCardFront rossz típus: {model.Tipus}");

                if (model.Tipus != DocumentTypes.DrivingLicenceBack) // Ha ennek ismertük fel, akkor elnézőek vagyunk az ocr tévesztés miatt
                    throw new WarningException("Wrong document type");
            }
        }

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanCreditCardFront
        });

        BookingFunctions.UpdateUtolsoVarazsloLepes(id, 8); // 7+1

        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public JsonResult ScanCreditCardBack(int id)
    {
        var model = IdScannerFunctionsInst.ScanCard();

        if (AppSettingsBase.GetAppSetting<int>("ScannedDocumentTypeValidation") != 0)
        {
            if (model.Tipus == DocumentTypes.Passport || model.Tipus == DocumentTypes.DrivingLicenceFront ||
                model.Tipus == DocumentTypes.IdCardFront) // || model.Tipus == DocumentTypes.DrivingLicenceBack || model.Tipus == DocumentTypes.IdCardBack
            {
                Log.Warning($"Foglalas/ScanCreditCardBack rossz típus: {model.Tipus}");
                throw new WarningException("Wrong document type");
            }
        }

        BookingFunctionsInst.UjCsomag(new DeliveryModel()
        {
            OrderId = id,
            ValueBytes = model.Kep,
            Type = DeliveryTypes.ScanCreditCardBack
        });

        //FoglalasModel foglalas = null;
        if (!BookingFunctions.FoglalasokMemory.TryGetValue(id, out FoglalasModel foglalas))
        {
            throw new Exception($"ScanCreditCardBack: No such reservation: {id}");
        }

        if (foglalas.Zarolando == 0)
        {
            BookingFunctionsInst.UjCsomag(new DeliveryModel()  // Ha nem kell deposit
            {
                OrderId = foglalas.Id,
                ValueInt = 1,
                ValueStr = "",
                ValueNyelv = foglalas.Nyelv,
                Type = DeliveryTypes.Deposit
            });

            if (foglalas.Fizetendo == 0)  // Ha a fizetés is már rendezve lett – különben a deposit részen figyeljük
            {
                BookingFunctionsInst.UjCsomag(new DeliveryModel()
                {
                    OrderId = foglalas.Id,
                    ValueInt = 1,
                    ValueStr = "",
                    ValueNyelv = foglalas.Nyelv,
                    Type = DeliveryTypes.Payment
                });
            }
        }


        return Json(new ResultModel() { Id = 0, Text = "" });
    }

    [HttpPost]
    public async Task<JsonResult> Leadas(string rendszam)
    {
        var result = await CrmFunctions.KocsiLeadas(rendszam);

        if (result != null)
        {
            var rekeszId = BookingFunctionsInst.SetTempValues(result.Id, result.RekeszIds);

            if (rekeszId == null)
                throw new WarningException("There is no free slot.");

            return Json(new { Id = result.Id, RekeszId = rekeszId });
        }
        throw new WarningException("No car with this license plate has been issued.");
    }

    [HttpPost]
    public JsonResult KulcsLeadas(int id, bool taxiFl)
    {
        byte rekeszId = BookingFunctionsInst.GetRekeszId(id) ?? 0;

        var result = CrmFunctions.KulcsLeadas(id, rekeszId, taxiFl);

        return Json(new ResultModel() { Id = (!result.Result).GetHashCode(), Text = "" });

        if (!KerongLockFunctions.IsLockClosed(rekeszId))
            throw new WarningException("Please close the compartment.");

        return Json(new ResultModel() { Id = 0, Text = "" });
    }
}
