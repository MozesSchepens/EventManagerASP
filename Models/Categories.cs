using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagerADV.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime? Deleted { get; set; } = DateTime.MaxValue;

        public virtual ICollection<Event>? Events { get; set; }
    }
}
