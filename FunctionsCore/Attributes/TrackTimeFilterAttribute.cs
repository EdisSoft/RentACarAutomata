using FunctionsCore.Contexts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Timers;

namespace FunctionsCore.Attributes
{
    public class TrackTimeFilter : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<string, Stopwatch> stopWatcherList = new ConcurrentDictionary<string, Stopwatch>();

        public static readonly ConcurrentDictionary<string, RequestData> RequestList = new ConcurrentDictionary<string, RequestData>();

        static FunctionsCore.Commons.Base.TrackTimeFilter TrackTimeFilterSettings;

        public static Action RunBefore { get; set; }
        public static Action<Stopwatch> RunAfter { get; set; }

        static int TrackTimeLog;
        static int TrackCountLog;

        public static Timer RequestLogWriter;
        static TrackTimeFilter()
        {
            TrackTimeFilterSettings = AppSettingsBase.GetTrackTimeFilterBeallitasok();
            TrackTimeLog = TrackTimeFilterSettings?.TrackTimeLog ?? -1;
            TrackCountLog = TrackTimeFilterSettings?.TrackCountLog ?? -1;
            if (TrackTimeFilterSettings.EnableRequestLog)
            {
                var interval = TrackTimeFilterSettings.RequestLogInterval * 1000;
                if (interval == 0)
                {
                    interval = 1000 * 60 * 60;
                }
                RequestLogWriter = new Timer(interval);
                RequestLogWriter.Enabled = true;
                RequestLogWriter.Elapsed += SaveRequestLog;
                RequestLogWriter.AutoReset = true;
                RequestLogWriter.Start();
            }
        }

        public static void SaveRequestLog(object sender, ElapsedEventArgs args)
        {
            StreamWriter sw=null;
            try
            {
                var fileName = $"{DateTime.Now.ToString("yyyyMMdd")}_RequestLog.csv";
                var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                sw = new StreamWriter($"{dir}/Logs/{fileName}", true);

                foreach (var item in RequestList.Where(x => x.Value.Url != null).ToList())
                {
                    sw.WriteLine($"{item.Value.StartDate:yyyy.MM.dd HH:mm:ss.fff};{item.Value.ElapsedMilliseconds};{item.Value.IP};{item.Value.Url}");
                    RequestList.TryRemove(item.Key, out var row);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Log.Error("SaveRequestLog hiba", ex);
                if (sw != null)
                {
                    sw.Close();
                }
            }

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                RunBefore?.Invoke();
                var stopWatch = new Stopwatch();
                stopWatcherList.TryAdd(filterContext.HttpContext.TraceIdentifier, stopWatch);
                if (TrackTimeFilterSettings.EnableRequestLog)
                {
                    RequestList.TryAdd(filterContext.HttpContext.TraceIdentifier, new RequestData()
                    {
                        IP = filterContext.HttpContext.Connection.RemoteIpAddress,
                        StartDate = DateTime.Now
                    });
                }
                stopWatch.Start();
            }
            catch (Exception e)
            {
                if (ElapsedTime.ErrorCounter >= ElapsedTime.ErrorCounterLimit)
                    return;

                ElapsedTime.ErrorCounter++;
                Log.Error($"Hiba az időmérés során!", e);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                stopWatcherList.TryGetValue(filterContext.HttpContext.TraceIdentifier, out var stopWatch);
                if (stopWatch == null)
                {
                    Log.Warning("StopWatch objektum megszűnt!");
                    return;
                }
                stopWatch.Stop();

                RunAfter?.Invoke(stopWatch);

                if (TrackTimeLog >= 0 && ElapsedTime.ErrorCounter < ElapsedTime.ErrorCounterLimit)
                    LogTime(filterContext.RouteData, filterContext, stopWatch.ElapsedMilliseconds, TrackTimeLog, TrackCountLog);
            }
            catch (Exception e)
            {
                if (ElapsedTime.ErrorCounter >= ElapsedTime.ErrorCounterLimit)
                    return;

                ElapsedTime.ErrorCounter++;
                Log.Error($"Hiba az időmérés során! Próbálkozás: {ElapsedTime.ErrorCounter}/10" + (ElapsedTime.ErrorCounter == 10 ? " - Ajjaj valami nagyon nem jó! Mára leállítva a TrackTime! Hiba:" : " - Hiba:"), e);

            }
            finally
            {
                Delete(filterContext?.HttpContext?.TraceIdentifier);
            }
        }

        public static void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            stopWatcherList.TryRemove(key, out var delete);
        }

        public static int Count()
        {
            if (stopWatcherList == null)
                return 0;
            return stopWatcherList.Count();
        }

        public static void Clear()
        {
            if (stopWatcherList != null)
                stopWatcherList.Clear();
        }

        private void LogTime(RouteData routeData, ActionExecutedContext context, long time, int configLimit, int configCount)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var type = context.HttpContext.Request.Method;
            var url = controllerName + "\\" + actionName + "\\" + type;
            if (TrackTimeFilterSettings.EnableRequestLog)
            {
                if (RequestList.TryGetValue(context.HttpContext.TraceIdentifier, out RequestData data))
                {
                    data.Url = url;
                    data.ElapsedMilliseconds = time;
                }
            }
            ElapsedTime.InsertEntity(time, url, configLimit, configCount);
        }


        private static class ElapsedTime
        {
            public static int ErrorCounter = 0;
            public const int ErrorCounterLimit = 10;
            public static ConcurrentDictionary<string, ElapsedTimeEntity> IdentifiedElapseds = new ConcurrentDictionary<string, ElapsedTimeEntity>();

            public static void InsertEntity(long milliseconds, string identifier, int configLimit, int configCount)
            {
                lock (IdentifiedElapseds)
                {
                    if (!IdentifiedElapseds.TryGetValue(identifier, out ElapsedTimeEntity entry))
                    {
                        entry = new ElapsedTimeEntity();
                        IdentifiedElapseds.TryAdd(identifier, entry);
                    }

                    if (entry.MinDate != default && entry.MinDate.Date < DateTime.Today)
                    {
                        IdentifiedElapseds = new ConcurrentDictionary<string, ElapsedTimeEntity>();
                        ErrorCounter = 0;
                    }

                    entry.LastTime = milliseconds;
                    entry.LastDate = DateTime.UtcNow;
                    entry.TotalTime += milliseconds;
                    entry.Count++;

                    if (milliseconds < entry.MinTime)
                    {
                        entry.MinTime = milliseconds;
                        entry.MinDate = DateTime.UtcNow;
                    }
                    if (milliseconds > entry.MaxTime)
                    {
                        entry.MaxTime = milliseconds;
                        entry.MaxDate = DateTime.UtcNow;
                    }

                    if (milliseconds >= configLimit && entry.Count >= configCount)
                        Log.Warning($"A függvény futási ideje nagyobb az elvártnál ({configLimit}ms): {identifier} - Átlagos futási idő az utolsó {entry.Count} alkalommal: {entry.Average}ms - min futási idő: {entry.MinTime}ms - jelenlegi futási idő: {milliseconds}ms - max futási idő: {entry.MaxTime}ms");
                }
            }
        }

        public class RequestData
        {
            public DateTime StartDate { get; set; }
            public long ElapsedMilliseconds { get; set; }
            public string Url { get; set; }

            public IPAddress IP { get; set; }
        }

        private class ElapsedTimeEntity
        {
            public long LastTime = 0;
            public long TotalTime = 0;
            public int Count = 0;
            public long MinTime = long.MaxValue;
            public long MaxTime = long.MinValue;
            public double Average => TotalTime / Count;

            public DateTime MinDate;
            public DateTime MaxDate;
            public DateTime LastDate;
        }
    }
}
