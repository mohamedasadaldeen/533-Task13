using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; }
        public string? Image { get; set; } = "actor.png";

    }
}
