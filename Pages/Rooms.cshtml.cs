using _24SevenBooking.Data;
using _24SevenBooking.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace _24SevenBooking.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly AppDbContext _context;

        public RoomsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; }

        public async Task OnGetAsync()
        {
            Rooms = await _context.Rooms.ToListAsync();
        }
    }
}
