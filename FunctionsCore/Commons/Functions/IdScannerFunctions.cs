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
using Pr22;
using Pr22.Task;
using Pr22.Processing;
using Microsoft.Extensions.Configuration;

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

        public IdScannerModel ScanCard()
        {
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

            IdScannerModel model = new IdScannerModel()
            {
                Nev = "Kovács Gábor",
                ErvenyessegVege = DateTime.Now.AddYears(2),
                OkmanyTipus = "IdCard",
                //Kep = page.Select(Pr22.Imaging.Light.White).GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Png).ToByteArray()
                Kep = page.Select(Pr22.Imaging.Light.White).GetImage().Save(Pr22.Imaging.RawImage.FileFormat.Jpeg).ToByteArray()
            };

            Log.Debug("Scanning processes are finished.");
            docReader.Close();

            return model;
        }
    }
}
