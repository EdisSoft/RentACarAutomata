using FunctionsCore.Contexts;
using FunctionsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionsCore.Commons.Functions
{
    public class KerongLockFunctions: IKerongLockFunctions
    {
        static string locksTcpAddress = "";
        static int locksTcpPort = 0;
        static byte cuBaseAddress = 0;

        TcpClient client = null;

        const byte FRAME_START = 2;
        const byte FRAME_END = 3;
        const byte CMD_GET_LOCKS_STATUS = 0x30;
        const byte CMD_OPEN_LOCK = 0x31;
        const byte CMD_OPEN_ALL_LOCKS = 0x33;

        public static void Init()
        {
            locksTcpAddress = AppSettingsBase.GetAppSetting("LockDriverTcpAddress");
            if (!Int32.TryParse(AppSettingsBase.GetAppSetting("LockDriverTcpPort"), out locksTcpPort))
            {
                locksTcpPort = 4001;
                Log.Debug("Invalid LockDriverTcpPort value in AppSettings " + AppSettingsBase.GetAppSetting("LockDriverTcpPort"));
            }
            if (!Byte.TryParse(AppSettingsBase.GetAppSetting("LockDriverCuAddress"), out cuBaseAddress))
            {
                cuBaseAddress = 0;
                Log.Debug("Invalid LockDriverCUAddress value in AppSettings " + AppSettingsBase.GetAppSetting("LockDriverCuAddress"));
            }
        }

        void Open()
        {
            try
            {
                Log.Debug("Opening TCP connection");
                // Connect to the LockDriver server
                client = new TcpClient(locksTcpAddress, locksTcpPort);
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
                throw;
            }
            Thread.Sleep(100);
        }

        void Close()
        {
            try
            {
                Log.Debug("Closing TCP connection");

                // Get a network stream for reading and writing data
                NetworkStream stream = client.GetStream();
                Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));
                stream.Close();

                // Disconnect to the LockDriver server
                client.Close();
                client.Dispose();
                client = null;
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
                throw;
            }
        }

        int SendCommand(byte address, byte command)
        {
            byte[] buffer = new byte[16];
            byte sum = 0;

            buffer[0] = FRAME_START;
            buffer[1] = (byte)address;
            buffer[2] = (byte)command;
            buffer[3] = FRAME_END;
            for (int i = 0; i < 4; i++)
            {
                sum += buffer[i];
            }
            buffer[4] = sum;

            // Get a network stream for reading and writing data
            NetworkStream stream = client.GetStream();
            Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));

            // Send the message to the connected TcpServer.
            stream.Write(buffer, 0, 5);

            stream.Flush();

            string txt = "";
            for (int i = 0; i < 5; i++)
            {
                txt += buffer[i].ToString("X2") + " ";
            }
            Log.Debug("Command sent: " + txt);

            Thread.Sleep(100);

            return 0;
        }

        bool ValidateFrame(byte[] data, int len)
        {
            byte sum = 0;

            if ((len < 5) || (data[0] != FRAME_START) || (data[len - 2] != FRAME_END))
            {
                return false;
            }
            for (int i = 0; i < len - 1; i++)
            {
                sum += data[i];
            }
            return (sum == data[len - 1]);
        }

        void EmptyStream()
        {
            // Buffer to store the response bytes.
            Byte[] data = new Byte[64];

            // Get a network stream for reading and writing data
            NetworkStream stream = client.GetStream();
            Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));

            while (stream.DataAvailable)
            {
                // Read the first batch of the TcpServer response bytes.
                stream.Read(data, 0, data.Length);
            }
        }

        Byte[] ReadData()
        {
            // Buffer to store the response bytes.
            Byte[] data = new Byte[64];

            // Get a network stream for reading and writing data
            NetworkStream stream = client.GetStream();
            Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));
            // Set timeout to 2000ms
            stream.ReadTimeout = 2000;

            /*
            while (!stream.DataAvailable)
            {
                Thread.Sleep(50);
            }*/

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            Log.Debug("" + bytes + " bytes data read");

            if (!ValidateFrame(data, bytes))
            {
                string txt = "Invalid data received: ";
                for (int i = 0; i < bytes; i++)
                {
                    txt += data[i].ToString("X2") + " ";
                }
                Log.Debug(txt);

                return new byte[0];
            }

            // String to store the response ASCII representation.
            //String responseData = String.Empty;
            //responseData = System.Text.Encoding.ASCII.GetString(data, 1, bytes - 2);
            Byte[] responseData = new byte[bytes - 3];
            Array.Copy(data, 1, responseData, 0, bytes - 3);
            //Console.WriteLine("Received: {0}", responseData);

            //return System.Text.Encoding.ASCII.GetBytes(responseData);
            return responseData;
        }

        public uint GetLocksStatus()
        {
            uint locksStatus = 0;

            // Clear earlier data from Stream
            EmptyStream();

            SendCommand((byte)(cuBaseAddress << 4), CMD_GET_LOCKS_STATUS);
            Thread.Sleep(50);

            byte[] data = ReadData();
            string txt = "";
            for (int i = 0; i < data.Length; i++)
            {
                txt += data[i].ToString("X2") + " ";
            }
            Log.Debug(txt);
            if (data.Length > 4)
            {
                locksStatus = BitConverter.ToUInt16(data, 2);
            }
            return locksStatus;
        }

        public void OpenLock(byte lockno)
        {
            if ((lockno < 1) || (lockno > 16))
            {
                throw new ArgumentOutOfRangeException(nameof(lockno), "value must be between 1 and 16");
            }
#if DEBUG
            return;
#endif
            Open();
            Log.Debug("Opening lock " + lockno);
            SendCommand((byte)((cuBaseAddress << 4) + (lockno - 1)), CMD_OPEN_LOCK);
            Thread.Sleep(50);
            Close();
            Thread.Sleep(500);
        }

        public bool IsLockClosed(byte lockno)
        {
            if ((lockno < 1) || (lockno > 16))
            {
                throw new ArgumentOutOfRangeException(nameof(lockno), "value must be between 1 and 16");
            }
            Open();
            Log.Debug("Reading locks status");
            uint locksStatus = GetLocksStatus();
            Close();
            Thread.Sleep(100);
            return ((locksStatus & (1 << (lockno - 1))) != 0);
        }

        void OpenAllLocks()
        {
            Log.Debug("Opening each locks");
            SendCommand(cuBaseAddress, CMD_OPEN_ALL_LOCKS);
            Thread.Sleep(100);
        }

        public void OpenCompartment(byte compNo)
        {
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug($"Opening Compartment: {compNo}");
            int lockNo = lockerAddresses.GetLockNumber(compNo);
            OpenLock((byte)lockNo);
        }

        public bool IsCompartmentClosed(byte compNo)
        {
            int lockNo = 0;
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug($"Reading Compartment state: {compNo}");
            //if ((lockno < 1) || (lockno > 16))
            //{
            //    throw new ArgumentOutOfRangeException(nameof(lockno), "value must be between 1 and 16");
            //}
            Log.Debug("Reading locks status");
            lockNo = lockerAddresses.GetLockNumber(compNo);
            return IsLockClosed((byte)lockNo);
        }

        public List<RekeszStatusModel> GetCompartmentStatuses()
        {
            uint locksStatuses;
            int lockNo;
            bool isOpen;
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug("Reading locks statuses");
            Open();
            locksStatuses = GetLocksStatus();
            Close();

            var compStatuses = new List<RekeszStatusModel>();
            for (int i = 1; i <= lockerAddresses.NumberOfCompartments; i++)
            {
                lockNo = lockerAddresses.GetLockNumber((byte)i);
                //((locksStatus & (1 << (lockno - 1))) != 0);
                isOpen = ((locksStatuses & (1 << (lockNo - 1))) == 0);
                // r.push({ RekeszId: i + 1, IsOpen: i % 2 == 0 });
                Log.Debug("{ " + $"RekeszId: {i}, IsOpen: {isOpen}" + " }");
                compStatuses.Add(new RekeszStatusModel() { RekeszId = i, IsOpen = isOpen });
            }
            return compStatuses;
        }


    }
}
