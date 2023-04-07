using NLog;
using System;
using System.Collections.Generic;

namespace FunctionsCore
{
    public static class Log
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        public static void Trace(string message, Exception ex)
        {
            Logger.Trace(ex, message);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Debug(string message, Exception ex)
        {
            Logger.Debug(ex, message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Info(string message, Exception ex)
        {
            Logger.Info(ex, message);
        }

        public static void Warning(string message)
        {
            Logger.Warn(message);
        }

        public static void Warning(string message, Exception ex)
        {
            Logger.Warn(ex, message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Error(string message, Exception ex)
        {
            Logger.Error(ex, message);
        }
        // for loging exception type, exception message etc..
        public static void Error(string message, string additionalMessage)
        {
            Logger.Error(additionalMessage, message);
        }

        public static void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public static void Fatal(string message, Exception ex)
        {
            Logger.Fatal(ex, message);
        }

    }
}
