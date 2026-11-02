using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cairo_Cinema.Models
{
    [PrimaryKey(nameof(MovieId), nameof(Image))]
    public class MovieSubImage
    {

        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Cinema { get; set; }
        public string Image {  get; set; }
        public ICollection<Movie> movie { get; set; }

    }
}
