using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using part1App.Models;

namespace part1App.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event/Index
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            ViewData["Venues"] = _context.Venue.ToList();
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venues"] = _context.Venue.ToList();
            return View(@event);
        }
        // GET: Event/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();
            ViewData["Venues"] = _context.Venue.ToList();
            return View(@event);
        }
        // POST: Event/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.EventId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Venues"] = _context.Venue.ToList();
            return View(@event);
        }
        // GET: Event/EditDetails/{id}
        // This action is for editing the event details
        // It includes the Venue data for the dropdown list
       

        // GET: Event/Details/{id}
        public async Task<IActionResult> EditDetails(int? id)
        {
            if (id == null) return NotFound();
            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return NotFound();
            return View(@event);
        }
        // POST: Event/EditDetails/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDetails(int id, Event @event)
        {
            if (id != @event.EventId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Venues"] = _context.Venue.ToList();
            return View(@event);
        }



        // GET: Event/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            // Check if the event exists
            if (@event == null) return NotFound();

            return View(@event);

        }
        // POST: Event/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

     

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }


        // GET: Event/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }
        // GET: Event/Bookings/{id} 

        public async Task<IActionResult> Bookings(int? id)
        {
            if (id == null) return NotFound();
            var bookings = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Where(b => b.EventId == id)
                .ToListAsync();
            if (bookings == null) return NotFound();
            return View(bookings);
        }  
       

    }
    


}
