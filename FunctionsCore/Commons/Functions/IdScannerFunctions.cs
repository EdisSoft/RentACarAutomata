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
using Microsoft.Extensions.Configuration;
using Pr22;
using Pr22.Task;
using Pr22.Processing;

namespace FunctionsCore.Commons.Functions
{
    public class IdScannerFunctions: IIdScannerFunctions
    {
        private static readonly object LockObject = new object();
        private DocumentReaderDevice docReader = null;
        private bool DocPresent = false;
        private IHttpRequestService requestService;

        public IdScannerFunctions(IHttpRequestService requestService, IConfiguration configuration)
        {
            this.requestService = requestService;
        }

        //----------------------------------------------------------------------
        /// <summary>
        /// Opens the first document reader device.
        /// </summary>
        /// <returns></returns>
        private int Open()
        {
            Log.Debug("Opening a scanner device");
            docReader = new DocumentReaderDevice();
            try
            {
                docReader = new Pr22.DocumentReaderDevice();
            }
            catch (Exception ex)
            {
                if (ex is DllNotFoundException || ex is Pr22.Exceptions.FileOpen)
                {
                    int platform = IntPtr.Size * 8;
                    int codepl = GetCodePlatform();

                    Log.Error("This sample program" + (codepl == 0 ? " is compiled for Any CPU and" : "") +
                        " is running on " + platform + " bit platform.\n" +
                        "Please check if the Passport Reader is installed correctly or compile your code for "
                        + (96 - platform) + " bit.\n" + ex.Message);
                }
                else
                {
                    Log.Error("Error: " + ex.Message);
                }
                throw new Exception("IdScanner.DllError");
            }

            docReader.Connection += onDeviceConnected;
            docReader.DeviceUpdate += onDeviceUpdate;

            try
            {
                docReader.UseDevice(0);
            }
            catch (Pr22.Exceptions.NoSuchDevice)
            {
                Log.Error("No device found!");
                throw new Exception("IdScanner.NoDevice");
            }

            //Subscribing to scan events
            docReader.ScanStarted += ScanStarted;
            docReader.ImageScanned += ImageScanned;
            docReader.ScanFinished += ScanFinished;
            docReader.DocFrameFound += DocFrameFound;
            docReader.PresenceStateChanged += PresentStateChanged;

            Log.Debug("The device " + docReader.DeviceName + " is opened.");
            return 0;
        }

        //----------------------------------------------------------------------
        private int GetCodePlatform()
        {
            System.Reflection.PortableExecutableKinds pek;
            System.Reflection.ImageFileMachine mac;
            System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.GetPEKind(out pek, out mac);

            if ((pek & System.Reflection.PortableExecutableKinds.PE32Plus) != 0)
            {
                return 64;
            }
            if ((pek & System.Reflection.PortableExecutableKinds.Required32Bit) != 0)
            {
                return 32;
            }
            return 0;
        }

        //----------------------------------------------------------------------
        /// <summary>
        /// Returns the document type name.
        /// </summary>
        /// <param name="doc_type">Document type identifier string.</param>
        /// <returns>The name of the document type.</returns>
        private static string GetDocTypeName(string doc_type)
        {
            if (doc_type.StartsWith("DL"))
            {
                if (doc_type == "DLL") return "driving license for learner";
                else return "driving license";
            }
            else if (doc_type.StartsWith("ID"))
            {
                if (doc_type == "IDF") return "ID card for foreigner";
                else if (doc_type == "IDC") return "ID card for children";
                else return "ID card";
            }
            else if (doc_type.StartsWith("PP"))
            {
                if (doc_type == "PPD") return "diplomatic passport";
                else if (doc_type == "PPS") return "service passport";
                else if (doc_type == "PPE") return "emergency passport";
                else if (doc_type == "PPC") return "passport for children";
                else return "passport";
            }
            else if (doc_type.StartsWith("TD")) return "travel document";
            else if (doc_type.StartsWith("RP")) return "residence permit";
            else if (doc_type.StartsWith("VS")) return "visa";
            else if (doc_type.StartsWith("WP")) return "work permit";
            else if (doc_type.StartsWith("SI")) return "social insurance document";
            else return "document";
        }

        //----------------------------------------------------------------------
        /// <summary>
        /// Returns the page name.
        /// </summary>
        /// <param name="doc_page">Document page identifier.</param>
        /// <returns>Document page name.</returns>
        private static string GetPageName(string doc_page)
        {
            if (doc_page == "F") return "front";
            else if (doc_page == "B") return "back";
            return "";
        }

        //----------------------------------------------------------------------
        public static string GetFieldValue(Pr22.Processing.Document Doc, Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = Doc.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                try
                {
                    string value = Doc.GetField(FR).GetBestStringValue();
                    if (value != "") return value;
                }
                catch (Pr22.Exceptions.EntryNotFound) { }
            }
            return "";
        }

        //----------------------------------------------------------------------
        public static string GetFieldRawValue(Pr22.Processing.Document Doc, Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = Doc.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                try
                {
                    string value = Doc.GetField(FR).GetRawStringValue();
                    if (value != "") return value;
                }
                catch (Pr22.Exceptions.EntryNotFound) { }
            }
            return "";
        }

        //----------------------------------------------------------------------
        public static string GetFieldFormatedValue(Pr22.Processing.Document Doc, Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = Doc.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                try
                {
                    string value = Doc.GetField(FR).GetFormattedStringValue();
                    if (value != "") return value;
                }
                catch (Pr22.Exceptions.EntryNotFound) { }
            }
            return "";
        }

        //----------------------------------------------------------------------
        public static string GetFieldStdValue(Pr22.Processing.Document Doc, Pr22.Processing.FieldId Id)
        {
            FieldReference filter = new FieldReference(FieldSource.All, Id);
            List<FieldReference> Fields = Doc.GetFields(filter);
            foreach (FieldReference FR in Fields)
            {
                try
                {
                    string value = Doc.GetField(FR).GetStandardizedStringValue();
                    if (value != "") return value;
                }
                catch (Pr22.Exceptions.EntryNotFound) { }
            }
            return "";
        }

        //----------------------------------------------------------------------
        // Event handlers
        //----------------------------------------------------------------------

        private void onDeviceConnected(object a, Pr22.Events.ConnectionEventArgs e)
        {
            Log.Debug("Connection event. Device number: " + e.DeviceNumber);
        }
        //----------------------------------------------------------------------

        private void onDeviceUpdate(object a, Pr22.Events.UpdateEventArgs e)
        {
            string str = "Update event.";
            switch (e.part)
            {
                case 1:
                    str += "  Reading calibration file from device.";
                    break;
                case 2:
                    str += "  Scanner firmware update.";
                    break;
                case 4:
                    str += "  RFID reader firmware update.";
                    break;
                case 5:
                    str += "  License update.";
                    break;
            }
            Log.Debug(str);
        }
        //----------------------------------------------------------------------

        private void ScanStarted(object a, Pr22.Events.PageEventArgs e)
        {
            Log.Debug("Scan started. Page: " + e.Page);
        }
        //----------------------------------------------------------------------

        private void ImageScanned(object a, Pr22.Events.ImageEventArgs e)
        {
            Log.Debug("Image scanned. Page: " + e.Page + " Light: " + e.Light);
            Pr22.Imaging.RawImage img = ((DocumentReaderDevice)a).Scanner.GetPage(e.Page).Select(e.Light).GetImage();
            // Saving scanned image to jpeg 
            img.Save(Pr22.Imaging.RawImage.FileFormat.Jpeg).Save($"IMG_{ DateTime.Now:yyyyMMddHHmmss}_" + e.Light + ".jpg");
        }
        //----------------------------------------------------------------------

        private void ScanFinished(object a, Pr22.Events.PageEventArgs e)
        {
            Log.Debug("Page scanned. Page: " + e.Page + " Status: " + e.Status);
        }
        //----------------------------------------------------------------------

        private void DocFrameFound(object a, Pr22.Events.PageEventArgs e)
        {
            Log.Debug("Document frame found. Page: " + e.Page);
        }
        //----------------------------------------------------------------------

        // To raise this event FreerunTask.Detection() has to be started.
        private void PresentStateChanged(object a, Pr22.Events.DetectionEventArgs e)
        {
            if (e.State == Pr22.Util.PresenceState.Present)
            {
                DocPresent = true;
            }
        }
        //----------------------------------------------------------------------

        private string GetName(Document OcrDoc)
        {
            string name = "";
            string str;

            name = GetFieldValue(OcrDoc, FieldId.Name);
            if (string.IsNullOrEmpty(name))
            {
                name = GetFieldValue(OcrDoc, FieldId.Surname);
                str = GetFieldValue(OcrDoc, FieldId.Surname2);
                if (!string.IsNullOrEmpty(str))
                {
                    name += $" {str}";
                }
                str = GetFieldValue(OcrDoc, FieldId.MiddleName);
                if (!string.IsNullOrEmpty(str))
                {
                    name += $" {str}";
                }
                str = GetFieldValue(OcrDoc, FieldId.Givenname);
                if (!string.IsNullOrEmpty(str))
                {
                    name += $" {str}";
                }
            }
            return name.Trim();
        }

        //----------------------------------------------------------------------

        private DateTime GetExpiryDate(Document OcrDoc)
        {
            DateTime date = DateTime.MinValue;
            string str;

            str = GetFieldValue(OcrDoc, FieldId.ExpiryDate);
            if (!string.IsNullOrEmpty(str))
            {
                if (!DateTime.TryParse(str, out date))
                {
                    Log.Debug("Invalid expiry date format: " + str);
                }
            }
            return date;
        }

        //----------------------------------------------------------------------

        private DocumentTypes GetDocType(Document OcrDoc)
        {
            string documentTypeName;

            int documentCode = OcrDoc.ToVariant().ToInt();
            documentTypeName = ""; // GetDocumentName(documentCode);

            if (documentTypeName == "")
            {
                string issue_country = GetFieldValue(OcrDoc, FieldId.IssueCountry);
                string issue_state = GetFieldValue(OcrDoc, FieldId.IssueState);
                string doc_type = GetFieldValue(OcrDoc, FieldId.DocType);
                string doc_page = GetFieldValue(OcrDoc, FieldId.DocPage);
                string doc_subtype = GetFieldValue(OcrDoc, FieldId.DocTypeDisc);

                //string tmpval = Pr22.Extension.CountryCode.GetName(issue_country);
                //if (tmpval != "") issue_country = tmpval;

                //documentTypeName = issue_country + new StrCon() + issue_state
                //    + new StrCon() + Pr22.Extension.DocumentType.GetDocTypeName(doc_type)
                //    + new StrCon("-") + Pr22.Extension.DocumentType.GetPageName(doc_page)
                //    + new StrCon(",") + doc_subtype;

                if (doc_type.StartsWith("DL"))
                {
                    if (doc_page == "B")
                    {
                        return DocumentTypes.DrivingLicenceBack;
                    }
                    else if (doc_page != "F")
                    {
                        Log.Debug($"Invalid Document type combination: {doc_type} {doc_page}");
                    }
                    return DocumentTypes.DrivingLicenceFront;
                }
                else if (doc_type.StartsWith("ID"))
                {
                    if (doc_page == "B")
                    {
                        return DocumentTypes.IdCardBack;
                    }
                    else if (doc_page != "F")
                    {
                        Log.Debug($"Invalid Document type combination: {doc_type} {doc_page}");
                    }
                    return DocumentTypes.IdCardFront;
                }
                else if (doc_type.StartsWith("PP"))
                {
                    return DocumentTypes.Passport;
                }
                //else if (doc_type.StartsWith("TD")) return "travel document";
                //else if (doc_type.StartsWith("RP")) return "residence permit";
                //else if (doc_type.StartsWith("VS")) return "visa";
                //else if (doc_type.StartsWith("WP")) return "work permit";
                //else if (doc_type.StartsWith("SI")) return "social insurance document";
                //else return "document";
                else if (!string.IsNullOrEmpty(doc_type))
                {
                    Log.Debug($"Unknown Document type: {doc_type} {doc_page}");
                }
            }

            return DocumentTypes.Empty;
        }

        //----------------------------------------------------------------------

        private int GetAuthenticity(Document OcrDoc)
        {
            string dull = GetFieldValue(OcrDoc, FieldId.DullCheck);

            try
            {
                return Convert.ToInt32(dull);
            }
            catch (Exception)
            { }
            return 0;
        }

        //----------------------------------------------------------------------

        public IdScannerModel ScanCard()
        {
#if DEBUG
            return new IdScannerModel()
            {
                Nev = "Kovács Gábor",
                ErvenyessegVege = DateTime.Now.AddYears(2),
                OkmanyTipus = DocumentTypes.IdCardFront,
                Kep = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 }
            };
#endif
            //Devices can be manipulated only after opening.
            if (Open() != 0)
            {
                throw new Exception("IdScanner.NoCard");
            }

            DocScanner Scanner = docReader.Scanner;
            Engine OcrEngine = docReader.Engine;
            string fileName = $"{DateTime.Now:yyyyMMddHHmmss}";

            // Start detection task, for automatic starting scanning
            //TaskControl LiveTask = Scanner.StartTask(FreerunTask.Detection());

            DocScannerTask ScanningTask = new DocScannerTask();
            // Scanning using all type of lights
            ScanningTask.Add(Pr22.Imaging.Light.All);

            //DocPresent = false;
            //System.Console.WriteLine("At this point, the user has to change the document on the reader.");
            //while (DocPresent == false)
            //{
            //    System.Threading.Thread.Sleep(100);
            //}

            Log.Debug("Scanning the images.");
            Page page = Scanner.Scan(ScanningTask, Pr22.Imaging.PagePosition.First);

            // MRZ
            Log.Debug("Reading all the field data of the Machine Readable Zone.");
            EngineTask MrzReadingTask = new EngineTask();
            //Specify the fields we would like to receive.
            MrzReadingTask.Add(FieldSource.Mrz, FieldId.All);
            Document MrzDoc = OcrEngine.Analyze(page, MrzReadingTask);

            //PrintDocFields(MrzDoc);
            //Returned fields by the Analyze function can be saved to an XML file:
            MrzDoc.Save(Document.FileFormat.Xml).Save($"DOC_{fileName}_MRZ.xml");

            // VIZ
            Log.Debug("Reading all the textual and graphical field data as well as " +
                "authentication result from the Visual Inspection Zone.");
            EngineTask VIZReadingTask = new EngineTask();
            VIZReadingTask.Add(FieldSource.Viz, FieldId.All);
            Document VizDoc = OcrEngine.Analyze(page, VIZReadingTask);

            //PrintDocFields(VizDoc);
            //Returned fields by the Analyze function can be saved to an XML file:
            VizDoc.Save(Document.FileFormat.Xml).Save($"DOC_{fileName}_VIZ.xml");

            Log.Debug("Saving whole document.");
            docReader.Engine.GetRootDocument().Save(Document.FileFormat.Zipped).Save($"DOC_{fileName}.zip");

            // Stop detection task
            //LiveTask.Stop();

            Log.Debug("MRZ_Name: " + GetFieldValue(MrzDoc, FieldId.Name));
            Log.Debug("VIZ_Name: " + GetFieldValue(VizDoc, FieldId.Name));
            Log.Debug("MRZ_Surname: " + GetFieldValue(MrzDoc, FieldId.Surname));
            Log.Debug("VIZ_Surname: " + GetFieldValue(VizDoc, FieldId.Surname));
            Log.Debug("MRZ_Givenname: " + GetFieldValue(MrzDoc, FieldId.Givenname));
            Log.Debug("VIZ_Givenname: " + GetFieldValue(VizDoc, FieldId.Givenname));
            Log.Debug("MRZ_ExpiryDate: " + GetFieldValue(MrzDoc, FieldId.ExpiryDate));
            Log.Debug("VIZ_ExpiryDate: " + GetFieldValue(VizDoc, FieldId.ExpiryDate));
            Log.Debug("MRZ_ExpiryDate_Std: " + GetFieldStdValue(MrzDoc, FieldId.ExpiryDate));
            Log.Debug("VIZ_ExpiryDate_Std: " + GetFieldStdValue(VizDoc, FieldId.ExpiryDate));
            Log.Debug("MRZ_DocType: " + GetFieldValue(MrzDoc, FieldId.DocType));
            Log.Debug("VIZ_DocType: " + GetFieldValue(VizDoc, FieldId.DocType));
            Log.Debug("MRZ_DocPage: " + GetFieldValue(MrzDoc, FieldId.DocPage));
            Log.Debug("VIZ_DocPage: " + GetFieldValue(VizDoc, FieldId.DocPage));
            Log.Debug("MRZ_B900: " + GetFieldValue(MrzDoc, FieldId.B900));
            Log.Debug("VIZ_B900: " + GetFieldValue(VizDoc, FieldId.B900));
            Log.Debug("MRZ_DullCheck: " + GetFieldValue(MrzDoc, FieldId.DullCheck));
            Log.Debug("VIZ_DullCheck: " + GetFieldValue(VizDoc, FieldId.DullCheck));



            Log.Debug("MRZ_: " + GetFieldValue(MrzDoc, FieldId.Name));
            Log.Debug("VIZ_: " + GetFieldValue(VizDoc, FieldId.Name));

            string name = GetName(MrzDoc);
            if (string.IsNullOrEmpty(name))
            {
                name = GetName(VizDoc);
            }

            DateTime dtExpiry = GetExpiryDate(MrzDoc);
            if (dtExpiry.Equals(DateTime.MinValue))
            {
                dtExpiry = GetExpiryDate(VizDoc);
            }

            int iAuth = GetAuthenticity(VizDoc);

            DocumentTypes documentType = GetDocType(MrzDoc);
            if (documentType == DocumentTypes.Empty)
            {
                documentType = GetDocType(VizDoc);
                if (documentType == DocumentTypes.Empty)
                {
                    documentType= DocumentTypes.Unknown;
                }
            }

            IdScannerModel model = new IdScannerModel()
            {
                Nev = name,
                ErvenyessegVege = dtExpiry,
                EredetisegValoszinusege = iAuth,
                OkmanyTipus = documentType,
                Kep = page.Select(Pr22.Imaging.Light.White).GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Jpeg).ToByteArray()
            };

            Log.Debug("Scanning processes are finished.");
            docReader.Close();

            return model;
        }
    }
}
