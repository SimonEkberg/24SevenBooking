using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _24SevenBooking.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }

        [Required]
        public int RoomNumber { get; set; }

        [Required]
        public RoomCategory Category { get; set; }

        public int Points { get; set; }
    }
}
