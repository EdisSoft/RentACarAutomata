using FunctionsCore.Models;

namespace FunctionsCore.Commons.Functions
{
    public interface IDeliveryFunctions
    {
        void UjCsomag(DeliveryModel csomag);

        void KuldesAsync();

        void Kuldes();
    }
}
