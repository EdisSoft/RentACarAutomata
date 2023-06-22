﻿using FunctionsCore;
using FunctionsCore.Commons.Functions;
using FunctionsCore.Contexts;
using FunctionsCore.Utilities;
using System;

namespace Automata.Functions
{
    public class MoneraTerminalFunctions
	{
		string comPort = AppSettingsBase.GetAppSetting("MoneraComPort");
		EcrWrapperDotNetMlib.EftTerminalZVT Terminal;
		static volatile EcrWrapperDotNetMlib.EftTerminalZVT PayingTerminal = null;
		string LatestReceipt;

		const string DAILY_TASK_DEFAULT_START_AT = "01:00";


		public void Init()
		{
			Terminal = new EcrWrapperDotNetMlib.EftTerminalZVT();
			LatestReceipt = "";

			string libinfo = "";
			Log.Debug("readlibinfo");
			Terminal.readLibInfo(ref libinfo);
			Log.Debug(libinfo);

			Terminal.setLocalDisplayEvent(displayEvent);
			Terminal.setLocalPrinterEvent(printEvent);
		}

		~MoneraTerminalFunctions()
		{
			Disconnect();
		}

		void displayEvent(string txt)
		{
			Log.Debug("Disp event ** " + txt);
		}

		void printEvent(string txt)
		{
			LatestReceipt = txt;
			Log.Debug("Print event ** " + txt);
		}

		public int ReadLibInfo(ref string libInfo)
		{
			return Terminal.readLibInfo(ref libInfo);
		}

		public int NormalPayment(int iAmount, string tran_id)
		{
			int rc;

			Log.Debug("NormalPayment " + iAmount + " " + tran_id);
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Payment(iAmount, tran_id);

			Log.Debug("NormalPayment finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public int DepositPayment(int iAmount, string tran_id)
		{
			int rc;

			Log.Debug("DepositPayment " + iAmount + " " + tran_id);
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Deposit(iAmount, tran_id);

			Log.Debug("DepositPayment finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public int CancelPayment()
		{
			int rc;

			Log.Debug("CancelPayment ");
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Cancellation();

			Log.Debug("CancelPayment finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public static int BreakPayment()
        {
			return SendBreak();
        }

		public int DailyClose()
		{
			int rc;

			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}
			rc = Settlement();

			Disconnect();
			return rc;
		}

		public int TMSCall()
		{
			int rc;

			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}
			rc = CallTMS();

			Disconnect();
			return rc;

		}

		int Connect()
		{
			int ret = Terminal.connect(comPort);
			//Console.WriteLine("connect (" + ComPort + ") =" + ret);
			return ret;
		}

		int Disconnect()
		{
			return Terminal.disconnect();
		}

		public static string GetErrorName(int rc)
		{
			return System.Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc);
		}

		public static int SendBreak()
		{
			if (PayingTerminal != null)
			{
				PayingTerminal.sendBreak();
				return (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_break;
			}
			// TODO: ide mas ertek kene, de nem talaltam jobbat a listaban
			return (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok;
		}

		public int Diagnosis(ref uint ext_result)
		{
			return Terminal.diagnosis(ref ext_result);
		}

		public int NetworkDiagnosis(ref uint ext_result, string lp)
		{
			return Terminal.networkDiagnosis(ref ext_result, lp);
		}

		int CallTMS()
		{
			// call this function every day to download terminal software updates from TMS host and send the terminal status (if TMS is installed)
			//you can specify the TMS host and IP but it is better if you call this function with empty string, in this case the terminal will use the pre-configured IP:port
			return Terminal.callTMS("");
		}

		int Settlement()
		{
			//call this function every night to close financial day
			return Terminal.settlement();
		}

		int CallPayment(string cent_amount, string tran_id, EcrWrapperDotNetMlib.PaymentType paymentType)
		{
			// clean receipt
			LatestReceipt = "";
			Log.Debug("MoneraTerminal Payment: " + cent_amount + ", " + tran_id + ", " + paymentType);
			// Store actual terminal to able to interrupt payment
			PayingTerminal = Terminal;
			int rc = Terminal.payment(cent_amount, tran_id, paymentType);
			// After payment clear value
			PayingTerminal = null;
			return rc;
		}

		int Payment(string cent_amount, string tran_id)
		{
			return CallPayment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_DEFAULT);
		}

		int Payment(int iAmount, string tran_id)
		{
			return Payment((100 * iAmount).ToString(), tran_id);
		}

		int Deposit(string cent_amount, string tran_id)
		{
			return CallPayment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_PREAUTH);
		}

		int Deposit(int iAmount, string tran_id)
		{
			return Deposit((100 * iAmount).ToString(), tran_id);
		}

		int Reservation(string cent_amount, string tran_id, ref bool isLesserAmount, ref string sApprovedAmount)
		{
			// TODO: megtudni mi ez a pumpa sorszam, es hogy kell hasznalni
			int pump_num = 1;

			return Terminal.reservation(cent_amount, tran_id, pump_num, ref isLesserAmount, ref sApprovedAmount);
		}

		int Completion(string cent_amount, string tran_id)
		{
			// TODO: megtudni mi ez a pumpa sorszam, es hogy kell hasznalni
			int pump_num = 1;
			return Terminal.completion(cent_amount, tran_id, pump_num);
		}

		int Cancellation()
		{
			// cancel latest successful payment
			return Terminal.cancellation();
		}

		public void SetLanguage(sbyte iLang)
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("Connect " + rc + " " + GetErrorName(rc));
				return;
			}
			Terminal.setLanguage(iLang);
			Disconnect();
		}

		public string GetReceipt()
		{
			return LatestReceipt;
		}

		public static void InitDailyTask()
        {
			TimeSpan startAt;
			string str = AppSettingsBase.GetAppSetting("PosTerminalDailyTaskStartAt");

			if (String.IsNullOrEmpty(str) || !TimeSpan.TryParse(str, out startAt))
            {
				startAt = TimeSpan.Parse(DAILY_TASK_DEFAULT_START_AT);
            }

			// Start timer
			new OncePerDayTimer(startAt, DailyTask, "PosTerminal daily task");

		}

		public static bool DailyTask()
        {
			MoneraTerminalFunctions MoneraTerminal;

			Log.Debug("DailyClose started");
			if (!BookingFunctions.VanAktivUgyfel())
			{
				int rc;

				MoneraTerminal = new MoneraTerminalFunctions();
				MoneraTerminal.Init();
				rc = MoneraTerminal.DailyClose();
				Log.Debug($"Daily close results {rc} {GetErrorName(rc)}");
				rc = MoneraTerminal.TMSCall();
				Log.Debug($"TMS call results {rc} {GetErrorName(rc)}");
				Log.Debug("DailyClose finished");
				return true;
			}
			Log.Debug("System might busy, VanAktivUgyfel");
			return false;
		}
	}
}
