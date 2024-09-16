using _24SevenBooking.Data;
using _24SevenBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24SevenBooking.Pages
{
    public class BookingsModel(AppDbContext context) : PageModel
    {
        private readonly AppDbContext _context = context;

        [BindProperty]
        public Booking Booking { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        public List<Room> Rooms { get; set; } = new List<Room>();

        public List<Booking> Bookings { get; set; } = new List<Booking>();

        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            Rooms = await _context.Rooms.ToListAsync();
            Bookings = await _context.Bookings.Include(b => b.Customer).Include(b => b.Room).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PersonalRegistrationNumber == Customer.PersonalRegistrationNumber);

            if (existingCustomer != null)
            {
                Booking.CustomerId = existingCustomer.CustomerId;
            }
            else
            {
                var newCustomer = new Customer
                {
                    Name = Customer.Name,
                    Email = Customer.Email,
                    Phone = Customer.Phone,
                    PersonalRegistrationNumber = Customer.PersonalRegistrationNumber,
                    LoyaltyPoints = 0
                };

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                Booking.CustomerId = newCustomer.CustomerId;
            }
            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Room booked successfully!";
            return RedirectToPage("/Index");
        }

        //Check for conflicting bookings, needs to be triggered upon choosing start date of a new booking.
        public bool CheckRoomAvailability(int roomId, DateTime startDate, DateTime endDate)
        {
            if (Bookings == null || !Bookings.Any())
            {
                return true;
            }

            var conflictingBooking = Bookings
                .Where(b => b.RoomId == roomId &&
                            ((b.StartDate <= endDate && b.StartDate >= startDate) ||
                             (b.EndDate >= startDate && b.EndDate <= endDate)))
                .FirstOrDefault();

            return conflictingBooking == null;
        }
    }
}
