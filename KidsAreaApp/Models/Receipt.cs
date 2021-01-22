using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidsAreaApp.Models
{
    public class Receipt
    {
        [Key]
        public int SerialKey { get; set; } 

        [NotMapped]
        public byte[] BarCode { get; set; }
    }
}