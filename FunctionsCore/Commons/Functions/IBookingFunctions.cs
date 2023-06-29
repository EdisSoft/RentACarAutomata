using FunctionsCore.Models;
using System.Collections.Generic;

namespace FunctionsCore.Commons.Functions
{
    public interface IBookingFunctions
    {
        void UjCsomag(DeliveryModel csomag);
        void KuldesAsync();
        void Kuldes();
        byte? SetTempValues(int foglalasId, List<byte> rekeszIds);
        byte? GetRekeszId(int foglalasId);
    }
}
