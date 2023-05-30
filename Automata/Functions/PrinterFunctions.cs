using FunctionsCore;
using FunctionsCore.Models;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Automata.Functions
{
    public class PrinterFunctions : IPrinterFunctions
    {
        public static string PrinterName { get; } = "SAM4S GIANT-100";
        // Manual feed and cut command for printing
        static string PrinterFeedAndCut { get; } = "\x1D\x56\x42\x60\n";
        // Number of characters on an 80mm wide printer roll
        static int PrinterWidth = 42;
             
        public bool PrintReceiptHun(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber)
        {
            DateTime now = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(AlignToCenter("GAME Rentacar Kft.") + "\n\n");
                sb.Append(AlignToCenter("Szerzödés szám:") + "\n\n");
                sb.Append(AlignToCenter(agreementNumber) + "\n\n");
                sb.Append(AlignToCenter("Rendszám:") + "\n\n");
                sb.Append(AlignToCenter(plateNumber) + "\n\n");
                sb.Append(AlignToCenter("Bérlet vége:") + "\n\n");
                sb.Append(AlignToCenter(endOfRental.ToString("yyyy.MM.dd. HH.mm")) + "\n\n");
                sb.Append(AlignToCenter("Kiállítva:") + "\n\n");
                sb.Append(AlignToCenter(now.ToString("yyyy.MM.dd. HH.mm")) + "\n\n\n\n");
                sb.Append(AlignToCenter("Letét vagy fizetés összege") + "\n\n");
                sb.Append(AlignToCenter($"{money} Ft") + "\n\n");
                sb.Append(AlignToCenter("Elöengedély száma:") + "\n\n");
                sb.Append(AlignToCenter(preAuthorizationNumber) + "\n\n\n\n");
                sb.Append(AlignToCenter("Nem adóügyi bizonylat") + "\n\n");
                sb.Append(AlignToCenter("Köszönjük a bérlést!") + "\n\n\n\n");
                sb.Append(AlignToCenter("Website: gamerentacar.com") + "\n\n");
                sb.Append(AlignToCenter("Email: info@gamerentacar.com") + "\n\n");
                sb.Append(AlignToCenter("Phone: +36 30 622 7959") + "\n\n");

                // Blank String to Print out properly
                //sb.Append("                                                                                                                  \n");
                // Feed and Cut
                sb.Append(PrinterFeedAndCut);

                RawPrinterHelper.SendStringToPrinter(PrinterName, sb.ToString());
            }
            catch (Exception ex)
            {
                //log
                //ex.Message
                Log.Error(ex.Message);
                return false;
            }

            return true;
        }

        public bool PrintReceiptEng(string agreementNumber, string plateNumber, DateTime endOfRental, int money, string preAuthorizationNumber)
        {
            DateTime now = DateTime.Now;
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(AlignToCenter("GAME Rentacar Ltd.") + "\n\n");
                sb.Append(AlignToCenter("Agreement number:") + "\n\n");
                sb.Append(AlignToCenter(agreementNumber) + "\n\n");
                sb.Append(AlignToCenter("Plate number:") + "\n\n");
                sb.Append(AlignToCenter(plateNumber) + "\n\n");
                sb.Append(AlignToCenter("End of rental:") + "\n\n");
                sb.Append(AlignToCenter(endOfRental.ToString("yyyy.MM.dd. HH.mm")) + "\n\n");
                sb.Append(AlignToCenter("Date:") + "\n\n");
                sb.Append(AlignToCenter(now.ToString("yyyy.MM.dd. HH.mm")) + "\n\n\n\n");
                sb.Append(AlignToCenter("Deposit or payment") + "\n\n");
                sb.Append(AlignToCenter($"{money} Ft") + "\n\n");
                sb.Append(AlignToCenter("Pre-authorization number:") + "\n\n");
                sb.Append(AlignToCenter(preAuthorizationNumber) + "\n\n\n\n");
                sb.Append(AlignToCenter("Non tax printout") + "\n\n");
                sb.Append(AlignToCenter("Thank you for your rental!") + "\n\n\n\n");
                sb.Append(AlignToCenter("Website: gamerentacar.com") + "\n\n");
                sb.Append(AlignToCenter("Email: info@gamerentacar.com") + "\n\n");
                sb.Append(AlignToCenter("Phone: +36 30 622 7959") + "\n");

                // Blank String to Print out properly
                //sb.Append("                                                                                                                  \n");
                // Feed and Cut
                sb.Append(PrinterFeedAndCut);

                RawPrinterHelper.SendStringToPrinter(PrinterName, sb.ToString());
            }
            catch (Exception ex)
            {
                //log
                //ex.Message
                Log.Error(ex.Message);
                return false;
            }

            return true;
        }

        //TID=MONERA02, ATH=656473 B, RETNUM = 001, RETTXT=ELFOGADVA, AMT = 1,00, DATE=2014.10.03 13:54:07, CNB=545548######0117, REFNO=39, ACQ=OTP BANK
        //CTYP=MasterCard, LOC = OTP - MONERA TEST TERMINAL, AID=A0000000041010, TC = 3B88BBFC, TRID = CTID_43866, 
        public bool PrintOtpResult(MoneraReceiptModel receipt)
        //public bool PrintOTPResult(string TID, string ATH, string RETNUM, string RETTXT, string AMT, string DATE, string CNB, string REFNO, string ACQ, string CTYP, string LOC, string AID, string TC, string TRID)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                
                sb.Append("TERMINÁL ID: " + receipt.TerminalID + "\n");
                sb.Append("ELSZÁMOLÓ: " + receipt.ACQ + "\n");
                sb.Append("KÁRTYATÍPUS: " + receipt.CardType + "\n");
                sb.Append("KÁRTYASZÁM: " + receipt.CardNum + "\n");
                sb.Append("ELADÁS - SALE" + "\n");
                sb.Append("IDÖ: " + receipt.DateTime + "\n");
                sb.Append("REF NO: " + receipt.RefNo + "\n");
                sb.Append("AID: " + receipt.AID + "\n");
                sb.Append("TC: " + receipt.TC + "\n");
                sb.Append("ENGEDÉLYSZÁM/AUTH CODE:" + "\n");
                sb.Append(AlignToCenter(receipt.AuthCode) + "\n");
                sb.Append("VÁLASZ/RESP: " + receipt.RetNum + "\n");
                sb.Append(AlignToCenter(receipt.RetText) + "\n");
                sb.Append("ÖSSZEG: " + receipt.Amount + " Ft\n");
                
                // Feed and Cut
                sb.Append(PrinterFeedAndCut);
                
                /*sb.Append("TERMINÁL ID: " + TID + "\n");
                sb.Append("ELSZÁMOLÓ: " + ACQ + "\n");
                sb.Append("KÁRTYATÍPUS: " + CTYP + "\n");
                sb.Append("KÁRTYASZÁM: " + CNB + "\n");
                sb.Append("ELADÁS - SALE" + "\n");
                sb.Append("IDÖ: " + DATE + "\n");
                sb.Append("REF NO: " + REFNO + "\n");
                sb.Append("AID: " + AID + "\n");
                sb.Append("TC: " + TC + "\n");
                sb.Append("ENGEDÉLYSZÁM/AUTH CODE:" + "\n");
                sb.Append(ATH + "\n");
                sb.Append("VÁLASZ/RESP: " + RETNUM + "\n");
                sb.Append(RETTXT + "\n");
                sb.Append("ÖSSZEG: " + AMT + "\n");*/

                /*sb.Append("1" + "\n");
                sb.Append("12" + "\n");
                sb.Append(AlignToRight("1") + "\n");
                sb.Append(AlignToRight("12") + "\n");
                sb.Append(AlignToCenter("1") + "\n");
                sb.Append(AlignToCenter("12") + "\n");
                sb.Append(AlignToEdge("ABCDE", "12345") + "\n");
                sb.Append(AlignToEdge("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "01234567890123456789") + "\n");
                sb.Append("\n\n\n\n");
                sb.Append("\x1D\x56\x01\n");
                sb.Append("1" + "\n");
                sb.Append("12" + "\n");
                sb.Append(AlignToRight("1") + "\n");
                sb.Append(AlignToRight("12") + "\n");
                sb.Append(AlignToCenter("1") + "\n");
                sb.Append(AlignToCenter("12") + "\n");
                sb.Append(AlignToEdge("ABCDE", "12345") + "\n");
                sb.Append(AlignToEdge("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "01234567890123456789") + "\n");
                sb.Append(PrinterFeedAndCut);*/
                
                //sb.Append("012345678901234567890123456789012345678901\n");
                //sb.Append("abcdefghijklmnopqrstuvwxyz áéíóöőúüű");
                //sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ ÁÉÍÓÖŐÚÜŰ\n");
                //sb.Append(".,;:'\"!?/\\@\n");
                //sb.Append("\x1D\x56\x01\n");
                //sb.Append("012345678901234567890123456789012345678901\n");
                //sb.Append("abcdefghijklmnopqrstuvwxyz áéíóöőúüű");
                //sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ ÁÉÍÓÖŐÚÜŰ\n");
                //sb.Append(".,;:'\"!?/\\@\n");
                //sb.Append(" \n");
                //sb.Append("\x1D\x56\x01\n");
                /*sb.Append("012345678901234567890123456789012345678901\n");
                sb.Append("abcdefghijklmnopqrstuvwxyz áéíóöőúüű");
                sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ ÁÉÍÓÖŐÚÜŰ\n");
                sb.Append(".,;:'\"!?/\\@\n");
                sb.Append(" \n");
                sb.Append(" \n");
                sb.Append("\x1D\x56\x01\n");
                sb.Append("012345678901234567890123456789012345678901\n");
                sb.Append("abcdefghijklmnopqrstuvwxyz áéíóöőúüű\n");
                sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ ÁÉÍÓÖŐÚÜŰ\n");
                sb.Append(".,;:'\"!?/\\@\n");
                sb.Append("\x1D\x56\x42\x20\n");
                sb.Append("012345678901234567890123456789012345678901\n");
                sb.Append("árvíztűrő tükörfúrógép\n");
                sb.Append("ÁRVÍZTŰRŐ TÜKÖRFÚRÓGÉP\n");
                sb.Append(".,;:'\"!?/\\@\n");
                sb.Append("\x1D\x56\x42\x40\n");*/

                RawPrinterHelper.SendStringToPrinter(PrinterName, sb.ToString());
            }
            catch (Exception ex)
            {
                //log
                //ex.Message
                Log.Error(ex.Message);
                return false;
            }

            return true;
        }

        private string AlignToCenter(string text)
        {
            if (text == null)
                text = "";

            var count = (PrinterWidth / 2) - (text.Length / 2);

            if (count > 0)
            {
                var shift = "".PadLeft(count, ' ');
                return shift + text;
            }

            return text;
        }

        private string AlignToRight(string text)
        {
            if (text == null)
                text = "";

            return text.PadLeft(PrinterWidth);
        }

        private string AlignToEdge(string left, string right)
        {
            if (left == null)
                left = "";
            if (right == null)
                right = "";

            if (left.Length + right.Length <  PrinterWidth)
            {
                return left + right.PadLeft(PrinterWidth - left.Length);
            }

            return left + "\n" + right.PadLeft(PrinterWidth);
        }

        /// <summary>
        /// Printer Control Class
        /// Reference from Microsoft
        /// </summary>
        public class RawPrinterHelper
        {
            // Structure and API declarions:
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }

            #region DllImport to Control the Printer

            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

            #endregion

            // SendBytesToPrinter()
            // When the function is given a printer name and an unmanaged array
            // of bytes, the function sends those bytes to the print queue.
            // Returns true on success, false on failure.
            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "GAME Rentacar számla";
                di.pDataType = "RAW";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }

                // If you did not succeed, GetLastError may give more information about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                    Log.Debug("Printing error: " + dwError);
                }

                return bSuccess;
            }

            /// <summary>
            /// Send String to Printer
            /// </summary>
            /// <param name="szPrinterName"></param>
            /// <param name="szString"></param>
            /// <returns></returns>
            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                Int32 dwCount = szString.Length;

                // Assume that the printer is expecting ANSI text, and then convert the string to ANSI text.
                IntPtr pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                var result = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                //Marshal.FreeHGlobal(pBytes);
                return result;
            }
        }
    }
}
