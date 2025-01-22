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
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser OrganisatorUser { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; }

        public string? DoneById { get; set; }
        [ForeignKey("DoneById")]
        public ApplicationUser? DoneBy { get; set; }

        public string? OrgId { get; set; }
        [ForeignKey("OrgId")]
        public ApplicationUser? OrgUser { get; set; }

        public DateTime BoDate { get; set; } = DateTime.UtcNow;

        public DateTime? Deleted { get; set; }

        public string? Remark { get; set; }
    }
}