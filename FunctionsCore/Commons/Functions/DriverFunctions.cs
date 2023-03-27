using FunctionsCore.Commons.Entities;
using FunctionsCore.Commons.EntitiesJson;

using FunctionsCore.Utilities.Extension.EnumExtension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionsCore.Commons.Functions
{
    public class DriverFunctions 
    {
        static sbyte mLanguage = 4; //0-German, 1-English, 4-Hungarian
        static String ctid = "";

        static void displayEvent(String txt)
        {
            Log.Info("Disp event " + DateTime.Now.ToString("HH:mm:ss.f") + "**" + txt);
        }

        static void printEvent(String txt)
        {
            Log.Info(txt);
            System.IO.StreamWriter outFile = new System.IO.StreamWriter("receipt.txt", true);
            outFile.WriteLine(txt);
            outFile.WriteLine("------------------------");
            outFile.Close();

        }


        public class BreakThread
        {
            //EftTerminalThales terminal;
            EcrWrapperDotNetMlib.EftTerminalZVT terminal;
            private volatile bool _shouldStop;

            //public BreakThread(EftTerminalThales terminal)
            public BreakThread(EcrWrapperDotNetMlib.EftTerminalZVT terminal)
            {
                this.terminal = terminal;
            }

            public void RequestStop()
            {
                _shouldStop = true;
            }

            // This method that will be called when the thread is started
            public void Run()
            {
                //while (true)
                {
                    Log.Info("Press a key to abort!");
                    while (Console.KeyAvailable == false && !_shouldStop)
                        Thread.Sleep(250); // Loop until input is entered.

                    if (!_shouldStop)
                    {
                        Console.ReadKey();
                        Log.Info("break signal");
                        terminal.sendBreak();
                    }
                    else
                        Log.Info("thread end");
                }
            }
        };

        static void Menu(EcrWrapperDotNetMlib.EftTerminalZVT terminal)
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            //string CardId = " ";
            int rc = 0;
            string amount = "0";
            BreakThread br;
            Thread th;
            uint ext_result;

            //terminal.setLanguage(mLanguage);
            while (key.KeyChar != 'x')
            {
                Log.Info("\npaym (p), cancel (c), read c. (r), PIN verify(v), lang (l), settl(s), tms call(t), netdiag(n), diag(d), reservation(e), completion(o), exit (x) ");
                //Log.Info("Mol functions: echo (0), read c. (1), authorization (2), financial advice (3), reconciliation (4), cancel (5)");

                key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case 'd':
                        Console.Write("device diagnosis");
                        ext_result = 0;
                        rc = terminal.diagnosis(ref ext_result);
                        Log.Info("result: " + ext_result);
                        if ((ext_result & (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_payment_not_possible) == (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_payment_not_possible)
                            Log.Info("Payment not possible!");
                        if ((ext_result & (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_COR) == (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_COR)
                            Log.Info("COR problem!");
                        if ((ext_result & (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_SCR) == (uint)EcrWrapperDotNetMlib.EDeviceProblems.E_SCR)
                            Log.Info("SCR problem!");

                        break;
                    case 'n':
                        Console.Write("ping (enter IP, eg. 192.168.1.1) or handshake (enter \"handshake\")");
                        String parameter = Console.ReadLine();
                        ext_result = 0;
                        rc = terminal.networkDiagnosis(ref ext_result, parameter);
                        Log.Info("netdiag res: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                        Log.Info("extended result: " + ext_result.ToString("X6"));
                        break;
                    case 't':// call this function every day to download terminal software updates from TMS host and send the terminal status (if TMS is installed)
                        rc = terminal.callTMS(""); //you can specify the TMS host and IP but it is better if you call this function with empty string, in this case the terminal will use the pre-configured IP:port
                        Log.Info("callTMS res: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                        break;
                    case 's': //call this function every night to close financial day
                        rc = terminal.settlement();
                        Log.Info("Settlement res: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                        break;
                    case 'l':
                        Log.Info("Choose Terminal Language (e - English, g - German, h - Hungarian)");
                        key = Console.ReadKey();
                        switch (key.KeyChar)
                        {
                            case 'h': mLanguage = 4; break;
                            case 'g': mLanguage = 0; break;
                            case 'e': mLanguage = 1; break;
                        }

                        terminal.setLanguage(mLanguage);
                        break;

                    case 'c':
                        Log.Info("Cancel");
                        rc = terminal.cancellation();
                        switch ((EcrWrapperDotNetMlib.ErrorCodes)rc)
                        {
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_ok:
                                Log.Info("\nCANCEL SUCCESSFUL!");
                                break;
                            default:
                                Log.Info("\nERR! #" + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                                break;

                        }
                        break;
                    case 'p': //normal Vending Machine case
                        Console.Write("\nAmount :");
                        amount = Console.ReadLine();

                        if (amount.IndexOf(",") == -1)
                            amount += "00";
                        else
                            amount = amount.Replace(",", "");

                        br = new BreakThread(terminal);
                        th = new Thread(new ThreadStart(br.Run));
                        th.Start();
                        while (!th.IsAlive) ;
                        Thread.Sleep(1);

                        {
                            Random random = new Random();
                            int randomNumber = random.Next(0, 100000);
                            ctid = "CTID_" + randomNumber; //max 24 chars
                        }

                        Log.Info("customer tran id: " + ctid);

                        rc = terminal.payment(amount, ctid, EcrWrapperDotNetMlib.PaymentType.PAY_DEFAULT);
                        switch ((EcrWrapperDotNetMlib.ErrorCodes)rc)
                        {
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_ok:
                                Log.Info("\nTRANSACTION SUCCESSFUL!");
                                /*System.IO.StreamWriter outFile = new System.IO.StreamWriter("receipt.txt", true);
                                outFile.WriteLine("\n------------------------");
                                outFile.Close();*/
                                break;


                            default:
                                Log.Info("\nERROR: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                                break;

                        }
                        break;

                    case 'e': //Fueld Dispensign Machine case, 1st round
                        Console.Write("\nAmount :");
                        amount = Console.ReadLine();

                        if (amount.IndexOf(",") == -1)
                            amount += "00";
                        else
                            amount = amount.Replace(",", "");

                        br = new BreakThread(terminal);
                        th = new Thread(new ThreadStart(br.Run));
                        th.Start();
                        while (!th.IsAlive) ;
                        Thread.Sleep(1);

                        //string ctid;
                        {
                            Random random = new Random();
                            int randomNumber = random.Next(0, 100000);
                            ctid = "CTID_" + randomNumber; //max 24 chars
                        }

                        Log.Info("customer tran id: " + ctid);

                        bool isLesserAmount = false;
                        String sApprovedAmount = "";

                        rc = terminal.reservation(amount, ctid, 1, ref isLesserAmount, ref sApprovedAmount);

                        switch ((EcrWrapperDotNetMlib.ErrorCodes)rc)
                        {
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_ok:
                                Log.Info("\nTRANSACTION SUCCESSFUL!");
                                if (isLesserAmount)
                                {
                                    UInt32 iAmt = UInt32.Parse(sApprovedAmount);
                                    Log.Info("Transaction performed lesser amount! " + (iAmt / 100).ToString() + "." + (iAmt % 100).ToString() + "HUF  Do you accept it? (Y/N)");
                                    System.ConsoleKeyInfo pressedKey = Console.ReadKey();
                                    if (pressedKey.Key == ConsoleKey.N)
                                    {
                                        //cancel the transaction
                                        Log.Info("Cancel the transaction...");
                                        terminal.completion("0", "123456789", 1);
                                    }
                                    else
                                    {
                                        Log.Info("Start fuel dispenser...");
                                        //start fuel dispenser
                                    }


                                }
                                /*System.IO.StreamWriter outFile = new System.IO.StreamWriter("receipt.txt", true);
                                outFile.WriteLine("\n------------------------");
                                outFile.Close();*/
                                break;


                            default:
                                Log.Info("\nERROR: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                                break;

                        }

                        br.RequestStop();
                        th.Join();

                        break;

                    case 'o': //Fueld Dispensign Machine case, 2nd round
                        Console.Write("\nAmount :");
                        amount = Console.ReadLine();

                        if (amount.IndexOf(",") == -1)
                            amount += "00";
                        else
                            amount = amount.Replace(",", "");

                        br = new BreakThread(terminal);
                        th = new Thread(new ThreadStart(br.Run));
                        th.Start();
                        while (!th.IsAlive) ;
                        Thread.Sleep(1);

                        Log.Info("customer tran id: " + ctid);

                        rc = terminal.completion(amount, ctid, 1);
                        ctid = "";

                        switch ((EcrWrapperDotNetMlib.ErrorCodes)rc)
                        {
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_ok:
                                Log.Info("\nTRANSACTION SUCCESSFUL!");
                                /*System.IO.StreamWriter outFile = new System.IO.StreamWriter("receipt.txt", true);
                                outFile.WriteLine("\n------------------------");
                                outFile.Close();*/
                                break;


                            default:
                                Log.Info("\nERROR: " + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                                break;

                        }

                        br.RequestStop();
                        th.Join();

                        break;


                    case 'r': //partner specific card handling implemented here (eg loyalty card)

                        //Log.Info("Please present card!");
                        String cardData = "";
                        rc = terminal.readCard(15, ref cardData, "300", 0);

                        switch ((EcrWrapperDotNetMlib.ErrorCodes)rc)
                        {
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_ok:
                                Log.Info("Card presented ");
                                Log.Info("Card data: " + cardData);

                                //in Mifare Classic case (where the SN is 4 bytes length) the SN is in LSB format
                                String serialNumHex = cardData.Substring(6, 2) + cardData.Substring(4, 2) + cardData.Substring(2, 2) + cardData.Substring(0, 2);
                                UInt32 sn = UInt32.Parse(serialNumHex, System.Globalization.NumberStyles.AllowHexSpecifier);
                                Log.Info("Mifare card SN: " + sn.ToString());

                                break;
                            case EcrWrapperDotNetMlib.ErrorCodes.VMC_crd_not_present: Log.Info("Card not inserted! "); break;

                            default:
                                Log.Info("ERROR #" + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                                break;


                        }

                        break;

                    case 'v': //partner specific pin handling implemented here (eg. pin verifycation for loyalty card)


                        break;


                    case 'S':
                        for (int i = 0; i < 5; i++)
                        {

                            //create some garbage
                            Version vt;
                            Random random = new Random();
                            int randomNumber = random.Next(800, 1000);

                            // Create objects and release them to fill up memory with unused objects. 
                            for (int j = 0; j < randomNumber; j++)
                            {
                                vt = new Version();
                            }

                            Log.Info("Start: " + DateTime.Now.ToString("HH:mm:ss.f"));
                            terminal.setLanguage(1);
                            /*ret = terminal.readCard(20);
                            Log.Info("readcard =" + ret);*/

                            //rc = terminal.payment("200", "hello");
                            String sAmt = "";
                            bool isLesAmt = false;

                            rc = terminal.reservation("7500", "1212", 1, ref isLesAmt, ref sAmt);
                            Log.Info("Lesser amt: " + isLesAmt + ", " + sAmt);
                            Log.Info("payment =" + rc + " " + Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc));
                            Log.Info("End: " + DateTime.Now.ToString("HH:mm:ss.f"));

                            terminal.completion("0", "", 1);

                            GC.Collect();
                        }
                        break;

                }
            }
        }


        public static void Main(string[] args)
        {
            Log.Info("Start program");
            EcrWrapperDotNetMlib.EftTerminalZVT terminal;
            terminal = new EcrWrapperDotNetMlib.EftTerminalZVT();

            string libinfo = "";
            Log.Info("readlibinfo");
            terminal.readLibInfo(ref libinfo);
            Log.Info(libinfo);

            terminal.setLocalDisplayEvent(displayEvent);
            terminal.setLocalPrinterEvent(printEvent);

            //string comPort = "COM1"; //or comPort = "192.168.1.1", CCV not support this
            string comPort = "192.168.10.51";
            if (args?.Length > 0)
                comPort = args[0];

            int ret = terminal.connect(comPort);
            Log.Info("connect (" + comPort + ") =" + ret);
            if (ret == 0)
                Menu(terminal);
            else
            {
                Log.Info("Press ENTER to EXIT");
                Console.ReadLine();
            }

            terminal.disconnect();
        }

    }
}
