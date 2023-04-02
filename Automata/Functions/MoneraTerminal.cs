using FunctionsCore;
//using System;

namespace Automata.Functions
{
    public class MoneraTerminal
    {
		string ComPort = "192.168.2.12";
		EcrWrapperDotNetMlib.EftTerminalZVT Terminal;
		string LatestReceipt;

		public MoneraTerminal(string aComPort)
		{
			ComPort = aComPort;
			Terminal = new EcrWrapperDotNetMlib.EftTerminalZVT();
			LatestReceipt = "";

			string libinfo = "";
			Log.Debug("readlibinfo");
			Terminal.readLibInfo(ref libinfo);
			Log.Debug(libinfo);

			Terminal.setLocalDisplayEvent(displayEvent);
			Terminal.setLocalPrinterEvent(printEvent);
		}



		~MoneraTerminal()
		{
			Disconnect();
		}

		void displayEvent(string txt)
		{
			Log.Debug("Disp event " + System.DateTime.Now.ToString("HH:mm:ss.f") + "** " + txt);
		}

		void printEvent(string txt)
		{
			LatestReceipt = txt;
			Log.Debug(txt);
			System.IO.StreamWriter outFile = new System.IO.StreamWriter("receipt.txt", true);
			outFile.WriteLine(txt);
			outFile.WriteLine("------------------------");
			outFile.Close();

		}

		public int ReadLibInfo(ref string libInfo)
		{
			return Terminal.readLibInfo(ref libInfo);
		}

		public int NormalPayment(int iAmount, string ctid)
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Payment(iAmount, ctid);
			return rc;
		}

		public int DepositPayment(int iAmount, string ctid)
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return rc;
			}

			rc = Deposit(iAmount, ctid);
			return rc;
		}

		public int DailyClose()
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return rc;
			}
			return Settlement();
		}

		public int TMSCall()
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return rc;
			}
			return CallTMS();

		}

		int Connect()
		{
			int ret = Terminal.connect(ComPort);
			//Console.WriteLine("connect (" + ComPort + ") =" + ret);
			return ret;
		}

		int Disconnect()
		{
			return Terminal.disconnect();
		}

		public string GetErrorName(int rc)
		{
			return System.Enum.GetName(typeof(EcrWrapperDotNetMlib.ErrorCodes), rc);
		}

		public void SendBreak()
		{
			Terminal.sendBreak();
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

		int Payment(string cent_amount, string tran_id)
		{
			// clean receipt
			LatestReceipt = "";
			return Terminal.payment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_DEFAULT);
		}

		int Payment(int iAmount, string tran_id)
		{
			// clean receipt
			LatestReceipt = "";
			string cent_amount = (100 * iAmount).ToString();
			return Terminal.payment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_DEFAULT);
		}

		public int Deposit(string cent_amount, string tran_id)
		{
			// clean receipt
			LatestReceipt = "";
			return Terminal.payment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_PREAUTH);
		}

		int Deposit(int iAmount, string tran_id)
		{
			// clean receipt
			LatestReceipt = "";
			string cent_amount = (100 * iAmount).ToString();
			return Terminal.payment(cent_amount, tran_id, EcrWrapperDotNetMlib.PaymentType.PAY_PREAUTH);
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

		public int Cancellation()
		{
			// cancel latest successful payment
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return rc;
			}
			return Terminal.cancellation();
		}

		public void SetLanguage(sbyte iLang)
		{
			int rc;
			rc = Disconnect();
			rc = Connect();
			if (rc != (int)EcrWrapperDotNetMlib.ErrorCodes.VMC_ok)
			{
				Log.Debug("\nConnect " + rc + " " + GetErrorName(rc));
				return;
			}
			Terminal.setLanguage(iLang);
		}

		public string GetReceipt()
		{
			return LatestReceipt;
		}
	}
}
