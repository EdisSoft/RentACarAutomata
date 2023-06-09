﻿using FunctionsCore.Contexts;
using FunctionsCore.Models;
using System;
using System.IO.Ports;
using System.Threading;

namespace FunctionsCore.Commons.Functions
{
    public class QrCodeReaderFunctions
	{
        SerialPort  serialPort;
        string      comPort;
        int         comSpeed;

        public void Init()
        {
            QrCodeReaderModel.Code = "";
            comPort = AppSettingsBase.GetAppSetting("QrCodeReaderComPort");
            if ( !Int32.TryParse(AppSettingsBase.GetAppSetting("QrCodeReaderComSpeed"), out comSpeed) )
            {
                comSpeed = 9600;
                Log.Debug("Invalid ComSpeed value in AppSettings " + AppSettingsBase.GetAppSetting("QrCodeReaderComSpeed"));
            }
        }

        public void Open()
        { 
            serialPort = new SerialPort(comPort, comSpeed, Parity.None, 8, StopBits.One);
            serialPort.Handshake = Handshake.None;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            serialPort.ReadTimeout = 250;
            serialPort.WriteTimeout = 250;
            try
            {
                // TODO error handling
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Log.Error("Error opening QRCode com port: " + ex.Message);
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            string data = serialPort.ReadExisting();
            QrCodeReaderModel.Code = data;
        }

        public void Close()
        {
            try
            { 
                serialPort.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Error closing QRCode com port: " + ex.Message);
            }
        }
    }
}
