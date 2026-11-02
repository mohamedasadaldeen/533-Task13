using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? Image { get; set; } = "poster.jpg";

    }
}
