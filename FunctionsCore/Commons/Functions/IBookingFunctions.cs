using FunctionsCore.Models;

namespace FunctionsCore.Commons.Functions
{
    public interface IBookingFunctions
    {
        void UjCsomag(DeliveryModel csomag);
        void KuldesAsync();
        void Kuldes();
        void SetTempValues(int foglalasId, int rekeszId);
        int GetRekeszId(int foglalasId);
    }
}
