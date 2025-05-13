using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using part1App.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace part1App.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenueController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Venue/Index
        public async Task<IActionResult> Index()
        {
            var venue = await _context.Venue.ToListAsync();
            return View(venue);
        }

        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                    venue.ImageUrl = blobUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venue/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venue/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingVenue = await _context.Venue.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                    if (existingVenue == null)
                        return NotFound();

                    if (venue.ImageFile != null)
                    {
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                        venue.ImageUrl = blobUrl;
                    }
                    else
                    {
                        venue.ImageUrl = existingVenue.ImageUrl;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(venue);
        }

        // GET: Venue/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // GET: Venue/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venue/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);

            if (venue == null)
                return NotFound();

            var bookings = await _context.Booking.Where(b => b.VenueId == id).ToListAsync();
            if (bookings.Count > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete venue with existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Venue/Bookings/{id}
        public async Task<IActionResult> Bookings(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venue/Bookings/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bookings(int id, Venue venue)
        {
            if (id != venue.VenueId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(venue);
        }

        // Helper: Check if venue exists
        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueId == id);
        }

        // Helper: Upload image to Azure Blob Storage
        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = _configuration["DefaultEndpointsProtocol=https;AccountName=cloudpoe2;AccountKey=Uv/gYxEapX77sq6T+V5PaO2YlfJWzbi2kuTSGHoE/VOktLuB7Z0hDQUsiElM0FOiSd6OFp302tv/+AStIZgqTw==;EndpointSuffix=core.windows.net"];
            var containerName = _configuration["cloudpoe"];

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, blobHttpHeaders);
            }

            return blobClient.Uri.ToString();
        }
    }
}
