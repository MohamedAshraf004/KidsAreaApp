using System.ComponentModel.DataAnnotations;

namespace KidsAreaApp.Models
{
    public class Hour
    {
        [Key]
        public int HourId { get; set; }
        [Required]
        public double HourPrice { get; set; }

    }
}