namespace FunctionsCore.Commons.Functions
{
    public interface IKerongLockFunctions
    {
        uint GetLocksStatus();
        void OpenLock(byte lockno);
        bool IsLockClosed(byte lockno);
    }
}
