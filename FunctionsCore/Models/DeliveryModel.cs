using FunctionsCore.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace FunctionsCore.Models
{
    public class DeliveryModel
    {
        [Key]
        public string Id
        {
            get
            {
                return $"{OrderId}-{Type}";
            }
        }
        public int OrderId { get; set; }
        public DeliveryTypes Type { get; set; }
        public string ValueStr { get; set; }
        public int ValueInt { get; set; }
        public byte[] ValueBytes { get; set; }
        public Nyelvek ValueNyelv { get; set; }
        public bool SendedFl { get; set; } = false;
        public byte NumberOfSending { get; set; } = 0;
    }
}