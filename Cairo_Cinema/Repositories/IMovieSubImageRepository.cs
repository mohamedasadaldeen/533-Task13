namespace Cairo_Cinema.Repositories
{
    public interface IMovieSubImageRepository : IRepository<MovieSubImage>
    {
        void RemoveRange(IEnumerable<MovieSubImage> movieSubImages);
    }
}
