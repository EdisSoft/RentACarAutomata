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
            int lockNo = 0;
            switch (iRekeszNo)
            {
                case 1:
                    lockNo = Lock01;
                    break;
                case 2:
                    lockNo = Lock02;
                    break;
                case 3:
                    lockNo = Lock03;
                    break;
                case 4:
                    lockNo = Lock04;
                    break;
                case 5:
                    lockNo = Lock05;
                    break;
                case 6:
                    lockNo = Lock06;
                    break;
                case 7:
                    lockNo = Lock07;
                    break;
                case 8:
                    lockNo = Lock08;
                    break;
            }
            return lockNo;
        }
    }

}