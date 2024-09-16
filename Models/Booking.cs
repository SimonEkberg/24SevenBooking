using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _24SevenBooking.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public bool IsCancelled { get; set; }

        public virtual Room Room { get; set; }
    }
}
