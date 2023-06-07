using FunctionsCore.Models;
using System.Collections.Generic;

namespace FunctionsCore.Commons.Functions
{
    public interface IBookingFunctions
    {
        void UjCsomag(DeliveryModel csomag);
        void KuldesAsync();
        void Kuldes();
        int SetTempValues(int foglalasId, List<int> rekeszIds);
        int GetRekeszId(int foglalasId);
    }
}
