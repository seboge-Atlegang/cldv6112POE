using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using part1App.Models;

namespace part1App.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Booking/Index
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings =  _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();
                
          

            // This method retrieves all bookings from the database, including related Event and Venue data, and passes them to the view.
            if(!String.IsNullOrEmpty(searchString))
            {
              
                bookings = bookings.Where(b => b.Event.Name.Contains(searchString) ||
                b.Venue.Name.Contains(searchString));
            }
            return View(await bookings.ToListAsync());
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            // Populate the dropdown lists for Venue and Event
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name");
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name");
            return View();
        }
        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

            // Check manually for double booking
            var conflict = await _context.Booking
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueId == booking.VenueId &&
                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking created successfully.";
                return RedirectToAction(nameof(Index));
            }
            // Repopulate the dropdown lists in case of validation failure
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }


        // Delete Booking (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)  // Include related Event data
                .Include(b => b.Venue)  // Include related Venue data
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // Delete Booking (POST) - Confirm Deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);  // Remove the booking
            await _context.SaveChangesAsync();  // Save changes to the database

            TempData["SuccessMessage"] = "Booking deleted successfully.";
            return RedirectToAction(nameof(Index));  // Redirect to the index view
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }

        // Edit Booking (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // POST: Booking/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "Name", booking.VenueId);
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "Name", booking.EventId);
            return View(booking);
        }

        // GET: Booking/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
    }
}