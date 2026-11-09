using Cairo_Cinema.Data;

namespace Cairo_Cinema.Repositories
{
    public class MovieSubImageRepository : Repository<MovieSubImage>, IMovieSubImageRepository
    {
        public MovieSubImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<MovieSubImage> movieSubImages)
        {
            _context.RemoveRange(movieSubImages);
        }
    }
}
