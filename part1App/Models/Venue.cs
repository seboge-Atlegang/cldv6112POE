using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace part1App.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Location { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }
        //Add this new one for updating the image from create and edit views
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public ICollection<Event> Events { get; set; }

    }
}