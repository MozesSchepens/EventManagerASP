using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagerASP.Models
{
    public class Organisator
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventId { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string OrgId { get; set; } = string.Empty;

        [Display(Name = "Added to Event")]
        public DateTime BoDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Added by")]
        public string? DoneById { get; set; }

        [Display(Name = "Remark")]
        public string? Remark { get; set; }

        public DateTime? Deleted { get; set; } = DateTime.MaxValue;

        // Navigatie-eigenschappen
        public virtual Event? Event { get; set; }
        public virtual ApplicationUser? OrganisatorUser { get; set; }
    }
}
