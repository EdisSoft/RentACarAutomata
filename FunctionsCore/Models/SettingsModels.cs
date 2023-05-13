namespace FunctionsCore.Models
{
    public class QueueTimings
    {
        public int MainQueueMinutes { get; set; }
    }
    public class LockerAddresses
    {
        public int Lock01 { get; set; }
        public int Lock02 { get; set; }
        public int Lock03 { get; set; }
        public int Lock04 { get; set; }
        public int Lock05 { get; set; }
        public int Lock06 { get; set; }
        public int Lock07 { get; set; }
        public int Lock08 { get; set; }

        public int GetLockNumber(int iRekeszNo)
        {
            switch (iRekeszNo)
            {
                case 1:
                    return Lock01;
                case 2:
                    return Lock02;
                case 3:
                    return Lock03;
                case 4:
                    return Lock04;
                case 5:
                    return Lock05;
                case 6:
                    return Lock06;
                case 7:
                    return Lock07;
                case 8:
                    return Lock08;
            }
            Log.Error($"Nem létező rekesz: {iRekeszNo}");
            return 0;
        }
    }

}