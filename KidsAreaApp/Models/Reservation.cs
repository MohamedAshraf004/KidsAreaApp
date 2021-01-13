using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsAreaApp.Models
{
    public class Reservation
    {
        public Reservation()
        {
            Receipt = new Receipt();
        }
        public int ReservationId { get; set; }
        public Receipt Receipt { get; set; }
        public DateTime StartReservationTme { get; set; } = DateTime.UtcNow;
        public DateTime EndReservationTme { get; set; } 
        public double Cost { get; set; }


    }
}
