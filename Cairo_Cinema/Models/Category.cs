using System.ComponentModel.DataAnnotations;

namespace Cairo_Cinema.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name{ get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public required ICollection<Movie> movie { get; set; }


    }
}
