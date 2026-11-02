using Cairo_Cinema.Models;

namespace Cairo_Cinema.ViewModel
{
    public class MovieVM
    {
        public Movie? Movie { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Cinema>? Cinemas { get; set; }
        public List<MovieSubImage>? MovieSubImages{ get; set; }
        public List<Actor>? Actors{ get; set; }

    }
}
