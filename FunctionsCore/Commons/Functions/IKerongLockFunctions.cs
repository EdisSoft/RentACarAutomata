using FunctionsCore.Models;
using System.Collections.Generic;

namespace FunctionsCore.Commons.Functions
{
    public interface IKerongLockFunctions
    {
        uint GetLocksStatus();
        bool OpenLock(byte lockno);
        bool IsLockClosed(byte lockno);
        bool OpenCompartment(byte compno);
        bool IsCompartmentClosed(byte compno);
        List<RekeszStatusModel> GetCompartmentStatuses();
    }
}
