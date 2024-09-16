using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _24SevenBooking.Data;
using _24SevenBooking.Models;
using _24SevenBooking.Controller;

namespace _24SevenBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(AppDbContext _context) : ControllerBase
    {
        private readonly CustomersController _customersController = new CustomersController(_context);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings/*.Include(b => b.Customer).Include(b => b.Room)*/.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }
            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PersonalRegistrationNumber == booking.Customer.PersonalRegistrationNumber);

            if (existingCustomer != null)
            {
                booking.CustomerId = existingCustomer.CustomerId;
            }
            else
            {
                var NewCustomer = new Customer
                {
                    Name = booking.Customer.Name,
                    Email = booking.Customer.Email,
                    Phone = booking.Customer.Phone,
                    PersonalRegistrationNumber = booking.Customer.PersonalRegistrationNumber,
                    LoyaltyPoints = 0
                };

                var result = await _customersController.PostCustomer(NewCustomer);
                if (result.Result is CreatedAtActionResult createdResult)
                {
                    var createdCustomer = (CustomerDto)createdResult.Value;
                    booking.CustomerId = createdCustomer.CustomerId;
                }
                else
                {
                    return BadRequest("Failed to create a new customer.");
                }
            }

            // Add the booking
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }

        [HttpGet("customers/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerDto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                LoyaltyPoints = customer.LoyaltyPoints
            };
            return Ok(customerDto);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
