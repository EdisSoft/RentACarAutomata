using FunctionsCore;
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

		const sbyte LANGUAGE_GER = 0;
		const sbyte LANGUAGE_ENG = 1;
		const sbyte LANGUAGE_HUN = 4;
		const string DAILY_TASK_DEFAULT_START_AT = "01:00";


		public void Init()
		{
			Terminal = new EcrWrapperDotNetMlib.EftTerminalZVT();
			LatestReceipt = "";

			string libinfo = "";
			Log.Info("Moneraterminal readlibinfo");
			Terminal.readLibInfo(ref libinfo);
			Log.Info(libinfo);

			Terminal.setLocalDisplayEvent(displayEvent);
			Terminal.setLocalPrinterEvent(printEvent);
		}

		~MoneraTerminalFunctions()
		{
			Disconnect();
		}

		void displayEvent(string txt)
		{
			Log.Info("Disp event ** " + txt);
		}

		void printEvent(string txt)
		{
			LatestReceipt = txt;
			Log.Info("Print event ** " + txt);
		}

		public int ReadLibInfo(ref string libInfo)
		{
			return Terminal.readLibInfo(ref libInfo);
		}

		public int NormalPayment(int iAmount, string tran_id)
		{
			int rc;

			Log.Info("NormalPayment " + iAmount + " " + tran_id);
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Payment(iAmount, tran_id);

			Log.Info("NormalPayment finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public int DepositPayment(int iAmount, string tran_id)
		{
			int rc;

			Log.Info("DepositPayment " + iAmount + " " + tran_id);
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Deposit(iAmount, tran_id);

			Log.Info("DepositPayment finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public int CancelPayment()
		{
			int rc;

			Log.Info("CancelPayment ");
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Cancellation();

			Log.Info("CancelPayment finished " + rc + " " + GetErrorName(rc));
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

			Log.Info("DailyClose");
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}
			rc = Settlement();

			Log.Info("DailyClose finished " + rc + " " + GetErrorName(rc));
			Disconnect();
			return rc;
		}

		public int TMSCall()
		{
			int rc;

			Log.Info("TMSCall");
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect error " + rc + " " + GetErrorName(rc));
				return rc;
			}
			rc = CallTMS();

			Log.Info("TMSCall finished " + rc + " " + GetErrorName(rc));
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
			Log.Info("SendBreak");
			if (PayingTerminal != null)
			{
				PayingTerminal.sendBreak();
				Log.Info("Break signal sent to active payment process");
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
			Log.Info("MoneraTerminal Payment: " + cent_amount + ", " + tran_id + ", " + paymentType);
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

		public void SetLanguageGer()
		{
			Log.Info("Set language to GERMAN");
			SetLanguage(LANGUAGE_GER);
		}

		public void SetLanguageEng()
		{
			Log.Info("Set language to ENGLISH");
			SetLanguage(LANGUAGE_ENG);
		}

		public void SetLanguageHun()
		{
			Log.Info("Set language to HUNGARIAN");
			SetLanguage(LANGUAGE_HUN);
		}

		public void SetLanguage(sbyte iLang)
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Error("Connect " + rc + " " + GetErrorName(rc));
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

			Log.Info("Initializing pos terminal daily task");
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

			Log.Info("DailyClose started");
			if (!BookingFunctions.VanAktivUgyfel())
			{
				int rc;

				MoneraTerminal = new MoneraTerminalFunctions();
				MoneraTerminal.Init();
				rc = MoneraTerminal.DailyClose();
				Log.Info($"Daily close results {rc} {GetErrorName(rc)}");
				rc = MoneraTerminal.TMSCall();
				Log.Info($"TMS call results {rc} {GetErrorName(rc)}");
				Log.Info("DailyClose finished");
				return true;
			}
			Log.Info("System might busy, VanAktivUgyfel");
			return false;
		}
	}
}
