using FunctionsCore.Models;
using System.Collections.Generic;

namespace FunctionsCore.Commons.Functions
{
    public interface IKerongLockFunctions
    {
        uint GetLocksStatus();
        void OpenLock(byte lockno);
        bool IsLockClosed(byte lockno);
        void OpenCompartment(byte compno);
        bool IsCompartmentClosed(byte compno);
        List<RekeszStatusModel> GetCompartmentStatuses();
    }
}
