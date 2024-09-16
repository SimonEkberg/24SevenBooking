using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _24SevenBooking.Data;
using _24SevenBooking.Models;

namespace _24SevenBooking.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(AppDbContext _context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            var customerDtos = customers.Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                LoyaltyPoints = c.LoyaltyPoints
            }).ToList();

            return Ok(customerDtos);
        }

        [HttpGet("{id}")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDto customerDto)
        {
            if (id != customerDto.CustomerId)
            {
                return BadRequest();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.Name = customerDto.Name;
            customer.Email = customerDto.Email;
            customer.Phone = customerDto.Phone;
            customer.LoyaltyPoints = customerDto.LoyaltyPoints;

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> PostCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.PersonalRegistrationNumber))
            {
                return BadRequest("Personal Registration Number is required.");
            }
            //TODO: Add agecheck
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Return DTO for hiding sensitive information. Could have a role-based check.
            var customerDto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                LoyaltyPoints = customer.LoyaltyPoints
            };

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customerDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
