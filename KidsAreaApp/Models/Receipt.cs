using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsAreaApp.Models
{
    public class Receipt
    {
        [Key]
        public Guid SerialKey { get; set; } = Guid.NewGuid();
        public string QrCodePath { get; set; }
        [NotMapped]
        public byte[] BarCode { get; set; }
    }
}