using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagerASP.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Naam")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Beschrijving")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Einddatum")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(2);

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required]
        [ForeignKey("Users")]
        public string StartedById { get; set; } = "?";

        public ApplicationUser? StartedByUser { get; set; }

        public DateTime Deleted { get; set; } = DateTime.MaxValue;
    }
}
