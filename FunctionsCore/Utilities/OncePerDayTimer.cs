using System;
using System.Threading;

namespace FunctionsCore.Utilities
{
    public class OncePerDayTimer : IDisposable
    {
        private DateTime? LastRunDate;
        private readonly TimeSpan Time;
        private Timer Timer;
        private readonly Action Callback;
        private readonly Func<bool> CallbackWithResult;

        public OncePerDayTimer(TimeSpan time, Action callback, string callbackString = "")
        {
            if (DateTime.Now.TimeOfDay > time)
            {
                LastRunDate = DateTime.Today;
            }
            Time = time;
            Timer = new Timer(CheckTime, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            Callback = callback;
            CallbackWithResult = null;

            Log.Info(string.Format("Ütemezett feladat bekötés sikeres. Minden nap: Idő: {0}, Feladat: {1}", time.ToString(), callbackString));
        }

        public OncePerDayTimer(TimeSpan time, Func<bool> callback, string callbackString = "")
        {
            if (DateTime.Now.TimeOfDay > time)
            {
                LastRunDate = DateTime.Today;
            }
            Time = time;
            Timer = new Timer(CheckTime, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            Callback = null;
            CallbackWithResult = callback;

            Log.Info(string.Format("Ütemezett feladat bekötés sikeres. Minden nap: Idő: {0}, Feladat: {1}", time.ToString(), callbackString));
        }

        private void CheckTime(object state)
        {
            if (LastRunDate == DateTime.Today)
                return;

            if (DateTime.Now.TimeOfDay < Time)
                return;

            if (Callback != null)
            {
                Callback();
                LastRunDate = DateTime.Today;
            }
            if ((CallbackWithResult != null) && CallbackWithResult())
            {
                LastRunDate = DateTime.Today;
            }
        }

        public void Dispose()
        {
            if (Timer == null)
                return;

            Timer.Dispose();
            Timer = null;
        }
    }

    public class OncePerWeekTimer : IDisposable
    {
        private DateTime? LastRunDate;
        private readonly TimeSpan Time;
        private readonly DayOfWeek Day;
        private Timer Timer;
        private readonly Action Callback;

        public OncePerWeekTimer(TimeSpan time, DayOfWeek day, Action callback, string callbackString = "")
        {
            if (DateTime.Now.DayOfWeek == day && DateTime.Now.TimeOfDay > time)
                LastRunDate = DateTime.Today;
            Time = time;
            Day = day;
            Timer = new Timer(CheckTime, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            Callback = callback;

            Log.Info(string.Format("Ütemezett feladat bekötés sikeres. Nap: {0}, Idő: {1}, Feladat: {2}", day.ToString(), time.ToString(), callbackString));
        }

        private void CheckTime(object state)
        {
            if (LastRunDate == DateTime.Today)
                return;

            if (DateTime.Now.DayOfWeek != Day)
                return;

            if (DateTime.Now.TimeOfDay < Time)
                return;

            LastRunDate = DateTime.Today;
            Callback();
        }

        public void Dispose()
        {
            if (Timer == null)
                return;

            Timer.Dispose();
            Timer = null;
        }
    }
}
