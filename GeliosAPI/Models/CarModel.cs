

using System.ComponentModel.DataAnnotations;

namespace GeliosAPI.Models
{
    public class CarModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }  

        [Required]
        public int Creator { get; set; }

        [Required]
        public string Name { get; set; }

        public string? LastDate { get; set; }
        
        public string? NextDate  { get; set; }

        [Required]
        public bool IsNameValid { get; set; }

        public bool? IsInspected { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
