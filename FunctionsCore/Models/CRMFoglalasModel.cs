using System;

namespace FunctionsCore.Models
{
    public class CrmFoglalasModel
    {
        public int orderID { get; set; }   
        public DateTime pickupdate { get; set; }
        public string bookingCode { get; set; }
        public DateTime dropoffdate { get; set; }
        public string rendszam { get; set; }
        public string keyid { get; set; }
        public string kontaktNev { get; set; }
        public string nev { get; set; }
        public string kontaktEmail { get; set; }
        public string locknumber { get; set; }
        public string deposit { get; set; }
        public string confirm_payment { get; set; }
        public string priceVehicle { get; set; }
        public string pickUpPrice { get; set; }
        public string dropOffPrice { get; set; }
        public string closedAtPickUpPrice { get; set; }
        public string closedAtDropOffPrice { get; set; }
        public string extras { get; set; }
        public string priceDiscount { get; set; }
        public string priceOver { get; set; }
        public string success_carried { get; set; }
        public bool? is_admin { get; set; }
        public string total_price { get; set; }
        public string type { get; set; }
    }
}
