using Microsoft.AspNetCore.Mvc;
namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;// = new Repository<Movie>();
        private readonly IRepository<Category> _categoryRepository;// = new Repository<Category>();
        private readonly IRepository<Cinema> _cinemaRepository;// = new Repository<Cinema>();
        private readonly IRepository<Actor> _actorRepository;// = new Repository<Actor>();
        private readonly IMovieSubImageRepository _movieSubImageRepository;// = new MovieSubImageRepository();

        public MovieController(
            IRepository<Movie> movieRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actor> actorRepository,
            IMovieSubImageRepository movieSubImageRepository
            )
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieSubImageRepository = movieSubImageRepository;
        }

        //Index
        public async Task<IActionResult> Index(FilterVM filter)
        {
            //movies = movies.Include(m => m.Category).Include(m => m.Actor).Include(m => m.Cinema);
            var movies = await _movieRepository.GetListAsync(includes:[m=>m.Actor, m=>m.Cinema, m => m.Category]);
            //filter
            if (filter._Price > 0)
            {
                movies = movies.Where(m => m.Price.Equals(filter._Price));
            }

            if (filter.RateId > 0)
            {
                movies = movies.Where(m => m.Rate >= filter.RateId);
            }

            if (filter.GenreId > 0)
            {
                movies = movies.Where(m => m.CategoryId == filter.GenreId);
            }

            if (filter.CinemaId > 0)
            {
                movies = movies.Where(m => m.CinemaId == filter.CinemaId);
            }

            ViewBag.Cinemas = await _cinemaRepository.GetListAsync();
            ViewBag.Categories = await _categoryRepository.GetListAsync();
            //Pagination
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 4.0);
            ViewBag.CurrentPage = filter.Page;
            movies = movies.Skip((filter.Page - 1) * 4).Take(4);
            return View(movies.AsEnumerable());
        }

        //Get Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetListAsync();
            var cinemas = await _cinemaRepository.GetListAsync();
            var actors = await _actorRepository.GetListAsync();

            return View(new MovieVM()
            {
                Actors = actors.ToList(),    
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
            });
        }

        //Post Create
        [HttpPost]
        public async Task<IActionResult> Create(MovieVM movieVM,IFormFile? ImageFile ,List<IFormFile>? MovieSubImages)
        {
            if (!ModelState.IsValid)
            {
                movieVM.Categories = (await _categoryRepository.GetListAsync()).ToList();
                movieVM.Cinemas = (await _cinemaRepository.GetListAsync()).ToList();
                movieVM.Actors = (await _actorRepository.GetListAsync()).ToList();
                return View(movieVM);
            }
            Movie movie = movieVM.Movie;
            //Image
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movie" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                movie.MainImage = fileName;
            }
            var savedMovie = await _movieRepository.AddAsync(movie);
            await _movieRepository.CommitAsync();
            //Images
            if (MovieSubImages != null && MovieSubImages.Count > 0)
            {
                foreach (var image in MovieSubImages)
                {
                    if(image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", fileName);
                        using(var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        await _movieSubImageRepository.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = savedMovie.Entity.Id
                        });
                    }
                   
                }
                await _movieRepository.CommitAsync();
            }
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
            var categories = await _categoryRepository.GetListAsync();
            var cinemas = await _cinemaRepository.GetListAsync();
            var actors = await _actorRepository.GetListAsync();
            var movieSubImages = await _movieSubImageRepository.GetListAsync();
            return View(new MovieVM()
            {
                Movie = movie,
                Actors = actors.ToList(),
                Categories = categories.ToList(),
                Cinemas = cinemas.ToList(),
                MovieSubImages = movieSubImages.ToList()
            });
        }

        //Post Update
        [HttpPost]
        public async Task<IActionResult> Update(Movie movie, IFormFile ImageFile, List<IFormFile> MovieSubImages)
        {
            var movieInDb = await _movieRepository.GetOneAsync(c => c.Id == movie.Id,isTracked:false);

            //MainImage
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movie", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                movie.MainImage = fileName;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movie", movieInDb.MainImage);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);

                }
            }
            else
            {
                movie.MainImage = movieInDb.MainImage;
            }
            _movieRepository.Update(movie);
            await _movieRepository.CommitAsync();
            //_________________________________________________________________________________
            //Images
            if (MovieSubImages != null && MovieSubImages.Count > 0)
            {
                var oldMovieSubImages = await _movieSubImageRepository.GetListAsync(m => m.MovieId == movie.Id);
                //Delete From wwwwroot
                foreach (var item in oldMovieSubImages)
                {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", item.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                }
                //Delete From DB
                _movieSubImageRepository.RemoveRange(oldMovieSubImages);
                await _movieRepository.CommitAsync();
            //_________________________________________________________________________________

                //Update MovieSubImages
                foreach (var image in MovieSubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        //Insert In wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        //Insert In DB
                        //Add Images In MovieSubImage Table
                        await _movieSubImageRepository.AddAsync(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = movie.Id
                        });
                    }
                }
            }
            await _movieRepository.CommitAsync();
            TempData["Successfully"] = "Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        //Delete SubImage
        public async Task<IActionResult> DeleteSubImage(int movieId, string Image)
        {
            var imageInDb = await _movieSubImageRepository.GetOneAsync(m=>m.MovieId == movieId && m.Image == Image);
            if (imageInDb != null)
            {
                var filPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", imageInDb.Image);
                if (System.IO.File.Exists(filPath))
                {
                    System.IO.File.Delete(filPath);
                }
                _movieSubImageRepository.Remove(imageInDb);
                await _movieSubImageRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Update), new {id = movieId});
        }
        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movie",movie.MainImage);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            // remove MovieSubImages from wwwroot
            var oldMovieSubImages = await _movieSubImageRepository.GetListAsync(p => p.MovieId == movie.Id);
            foreach (var item in oldMovieSubImages)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", item.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

            }
            _movieRepository.Remove(movie);
            await _movieRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
