﻿using FunctionsCore.Contexts;
using FunctionsCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace FunctionsCore.Commons.Functions
{
    public class KerongLockFunctions: IKerongLockFunctions
    {
        static string locksTcpAddress = "";
        static int locksTcpPort = 0;
        static byte cuBaseAddress = 0;

        static TcpClient client = null;
        static bool busy = false;           // TODO: using?

        const byte FRAME_START = 2;
        const byte FRAME_END = 3;
        const byte CMD_GET_LOCKS_STATUS = 0x30;
        const byte CMD_OPEN_LOCK = 0x31;
        const byte CMD_OPEN_ALL_LOCKS = 0x33;
        const byte TRY_COUNT_OPEN_LOCK = 3;
        const byte TRY_COUNT_CMD_SEND = 5;

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

            Log.Debug("Creating TCP client");
            client = null;
        }

        string FrameToString(byte[] data, int len = 0)
        {
            string txt = "";

            if (data != null)
            {
                if (len == 0)
                {
                    len = data.Length;
                }
                for (int i = 0; i < len; i++)
                {
                    txt += data[i].ToString("X2") + " ";
                }
            }
            return txt;
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
            Thread.Sleep(10);
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
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e);
                throw;
            }
        }

        int SendCommand(byte[] command)
        {
            Log.Debug("LockFunctions: Sending Command: " + FrameToString(command));

            bool bSucceed = false;
            int iTryCount = TRY_COUNT_CMD_SEND;

            while(!bSucceed && (iTryCount > 0))
            {
                iTryCount--;

                // OpenConnection();
                if (client == null)
                {
                    try
                    {
                        Log.Debug("Creating TCP client");
                        client = new TcpClient(locksTcpAddress, locksTcpPort);
                    }
                    catch (SocketException ex)
                    {
                        Log.Error("Error: SocketEx: (" + ex.SocketErrorCode + ") " + ex);
                        client.Close();
                        client = null;
                        if (iTryCount == 0)
                        {
                            throw;
                        }
                        continue;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error: " + e);
                        client.Close();
                        client = null;
                        if (iTryCount == 0)
                        {
                            throw;
                        }
                        continue;
                    }
                }

                //WriteCommand();
                try
                {
                    // Get a network stream for reading and writing data
                    NetworkStream stream = client.GetStream();
                    Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));
                    // Send the message to the connected TcpServer.
                    stream.Write(command, 0, command.Length);

                    Log.Debug("LockFunctions: Command sent");
                    bSucceed = true;
                }
                catch(SocketException e)
                {
                    Log.Error("LockFunctions: Sending error: (" + e.SocketErrorCode + ") " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }
                catch(InvalidOperationException e)
                {
                    Log.Error("LockFunctions: Sending error: " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }
                catch(IOException e) 
                {
                    Log.Error("LockFunctions: Sending error: " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }
            }

            if (!bSucceed)
            {
                Log.Debug("Sending command failed");
            }
            Thread.Sleep(10);

            return 0;
        }

        bool SendCommandAndRead(byte[] command, out byte[] responseData)
        {
            responseData = null;

            Log.Debug("LockFunctions: Sending Command: " + FrameToString(command));

            bool bSucceed = false;
            int iTryCount = TRY_COUNT_CMD_SEND;

            while(!bSucceed && (iTryCount > 0))
            {
                iTryCount--;

                // OpenConnection();
                if (client == null)
                {
                    try
                    {
                        Log.Debug("Creating TCP client");
                        client = new TcpClient(locksTcpAddress, locksTcpPort);
                    }
                    catch (SocketException ex)
                    {
                        Log.Error("Error: SocketEx: (" + ex.SocketErrorCode + ") " + ex);
                        client.Close();
                        client = null;
                        if (iTryCount == 0)
                        {
                            throw;
                        }
                        continue;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error: " + e);
                        client.Close();
                        client = null;
                        if (iTryCount == 0)
                        {
                            throw;
                        }
                        continue;
                    }
                }

                EmptyStream();

                //WriteCommand();
                try
                {
                    // Get a network stream for reading and writing data
                    NetworkStream stream = client.GetStream();
                    Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));
                    // Send the message to the connected TcpServer.
                    stream.Write(command, 0, command.Length);

                    Log.Debug("LockFunctions: Command sent");
                    Thread.Sleep(10);
                }
                catch(SocketException e)
                {
                    Log.Error("LockFunctions: Sending error: (" + e.SocketErrorCode + ") " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }
                catch(InvalidOperationException e)
                {
                    Log.Error("LockFunctions: Sending error: " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }
                catch(IOException e) 
                {
                    Log.Error("LockFunctions: Sending error: " + e.Message);
                    client.Close();
                    client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }

                // ReadResponse
                Byte[] data = new Byte[64];
                Int32 bytes = 0;
                try
                {
                    // Buffer to store the response bytes.

                    // Get a network stream for reading and writing data
                    NetworkStream stream = client.GetStream();
                    Log.Debug("Stream: " + stream.GetHashCode().ToString("X8"));
                    // Set timeout to 2000ms
                    stream.ReadTimeout = 2000;

                    // Read the first batch of the TcpServer response bytes.
                    bytes = stream.Read(data, 0, data.Length);
                    Log.Debug("" + bytes + " bytes data read");
                }
                catch (IOException e)
                {
                    Log.Error("LockFunctions: Receive error: " + e.Message);
                    // TODO: timeout felismerese? (InnerException-ben SocketErrorCode)
                    continue;
                }
                catch (Exception e)
                {
                    Log.Error("LockFunctions: Receive error: " + e.Message);
                    //client.Close();
                    //client = null;
                    if (iTryCount == 0)
                    {
                        throw;
                    }
                    continue;
                }

                // ValidateResponse
                if (!ValidateFrame(data, bytes))
                {
                    Log.Debug("Invalid data received: " + FrameToString(data, bytes));
                    continue;
                }

                responseData = new byte[bytes - 3];
                Array.Copy(data, 1, responseData, 0, bytes - 3);

                bSucceed = true;

            }

            if (!bSucceed)
            {
                responseData = new byte[0];
                Log.Debug("Sending command and receive failed");
            }
            Thread.Sleep(10);

            return bSucceed;
        }

        byte[] BuildCommand(byte address, byte command)
        {
            byte[] buffer = new byte[5];
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

            return buffer;
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
            if ((client == null) || (!client.Connected))
            {
                return;
            }

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

        public uint GetLocksStatus()
        {
            uint locksStatus = 0;

            byte[] data;
            byte[] command = BuildCommand((byte)(cuBaseAddress << 4), CMD_GET_LOCKS_STATUS);

            if (SendCommandAndRead(command, out data) == true)
            {
                Log.Debug(FrameToString(data));
                if (data.Length > 4)
                {
                    locksStatus = BitConverter.ToUInt16(data, 2);
                }
                return locksStatus;
            }
            return 0xFFFF;
        }

        public bool OpenLock(byte lockno)
        {
            if ((lockno < 1) || (lockno > 16))
            {
                throw new ArgumentOutOfRangeException(nameof(lockno), "value must be between 1 and 16");
            }
#if DEBUG
            return true;
#endif
            Log.Debug("Opening lock " + lockno);

            bool bSucceed = false;
            int iTryCount = TRY_COUNT_OPEN_LOCK;
            byte[] command = BuildCommand((byte)((cuBaseAddress << 4) + (lockno - 1)), CMD_OPEN_LOCK);

            while (!bSucceed && (iTryCount > 0))
            {
                iTryCount--;

                SendCommand(command);
                // wait some time till opening lock (maybe unnecessary)
                Thread.Sleep(500);
                bSucceed = !IsLockClosed(lockno);
            }
            return bSucceed;
        }

        public bool IsLockClosed(byte lockno)
        {
            if ((lockno < 1) || (lockno > 16))
            {
                throw new ArgumentOutOfRangeException(nameof(lockno), "value must be between 1 and 16");
            }

            Log.Debug("Reading locks status");

#if DEBUG
            return true;
#endif
            uint locksStatus = GetLocksStatus();
            return ((locksStatus & (1 << (lockno - 1))) != 0);
        }

        void OpenAllLocks()
        {
            Log.Debug("Opening each locks");
            byte[] command = BuildCommand((byte)(cuBaseAddress << 4), CMD_OPEN_ALL_LOCKS);
            SendCommand(command);
            Thread.Sleep(1000);
        }

        public bool OpenCompartment(byte compNo)
        {
#if DEBUG
            return true;
#endif
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug($"Opening Compartment: {compNo}");
            int lockNo = lockerAddresses.GetLockNumber(compNo);
            return OpenLock((byte)lockNo);
        }

        public bool IsCompartmentClosed(byte compNo)
        {
            int lockNo = 0;
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug($"Reading Compartment state: {compNo}");
            lockNo = lockerAddresses.GetLockNumber(compNo);
            return IsLockClosed((byte)lockNo);
        }

        public List<RekeszStatusModel> GetCompartmentStatuses()
        {
            string stateLog = "";
            uint locksStatuses;
            int lockNo;
            bool isOpen;
            var lockerAddresses = AppSettingsBase.GetLockerAddresses();

            Log.Debug("Reading locks statuses");
            //Open();
            locksStatuses = GetLocksStatus();
            //Close();

            var compStatuses = new List<RekeszStatusModel>();
            for (int i = 1; i <= lockerAddresses.NumberOfCompartments; i++)
            {
                lockNo = lockerAddresses.GetLockNumber((byte)i);
                //((locksStatus & (1 << (lockno - 1))) != 0);
                isOpen = ((locksStatuses & (1 << (lockNo - 1))) == 0);
                // r.push({ RekeszId: i + 1, IsOpen: i % 2 == 0 });
                stateLog += $"{i}: {isOpen},";
                compStatuses.Add(new RekeszStatusModel() { RekeszId = i, IsOpen = isOpen });
            }
            Log.Info($"GetCompartmentStatuses: {stateLog}");
            return compStatuses;
        }


    }
}
