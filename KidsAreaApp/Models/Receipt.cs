using System;
using System.ComponentModel.DataAnnotations;

namespace KidsAreaApp.Models
{
    public class Receipt
    {
        [Key]
        public Guid SerialKey { get; set; } = Guid.NewGuid();
        public string QrCodePath { get; set; }
        public byte[] BarCode { get; set; }
    }
}