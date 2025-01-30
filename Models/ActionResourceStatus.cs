using System.ComponentModel.DataAnnotations;

namespace DevOps.Models
{
    public class ActionResourceStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Action { get; set; }

        [Required]
        public string Resource { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
