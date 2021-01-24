using KidsAreaApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsAreaApp.Models
{
    public class Reservation
    {
     
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SerialKey { get; set; }
        [DisplayName("Start Time")]
        public DateTime StartReservationTme { get; set; } = DateTime.Now;
        [DisplayName("End Time")]
        public DateTime EndReservationTme { get; set; } = new DateTime(2021, 1, 1, 12, 0, 0);
        public double TotatCost { get; set; }
        public double CostAfterDiscount { get; set; }
        public double Discount { get; set; } = 0;
        [NotMapped]
        public byte[] BarCode { get; set; }
        [NotMapped]
        public int Index{ get; set; }


    }
}
