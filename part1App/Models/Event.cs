﻿using System.ComponentModel.DataAnnotations;

namespace part1App.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int VenueId { get; set; }

        // Navigation property for the related Venue
        public Venue? Venue { get; set; }
    }
}