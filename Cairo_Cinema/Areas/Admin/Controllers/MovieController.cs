using Cairo_Cinema.Data;
using Cairo_Cinema.Models;
using Cairo_Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        //Index
        public IActionResult Index(FilterVM filter)
        {
            var movies = _context.Movies.AsQueryable();
            //filter
            movies = movies.Include(m => m.Category).Include(m => m.Actor).Include(m => m.Cinema);
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

            ViewBag.Cinemas = _context.Cinemas;
            ViewBag.Categories = _context.Categories;
            //Pagination
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 4.0);
            ViewBag.CurrentPage = filter.Page;
            movies = movies.Skip((filter.Page - 1) * 4).Take(4);
            return View(movies.AsEnumerable());
        }

        //Get Create
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var cinemas = _context.Cinemas.ToList();
            var actors = _context.Actors.ToList();

            return View(new MovieVM()
            {
                Actors = actors,
                Categories = categories,
                Cinemas = cinemas,
            });
        }

        //Post Create
        [HttpPost]
        public IActionResult Create(MovieVM movieVM,IFormFile? ImageFile ,List<IFormFile>? MovieSubImages)
        {
            if (!ModelState.IsValid)
            {
                movieVM.Categories = _context.Categories.ToList();
                movieVM.Cinemas = _context.Cinemas.ToList();
                movieVM.Actors = _context.Actors.ToList();
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
            var savedMovie = _context.Movies.Add(movie);
            _context.SaveChanges();
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
                        _context.MovieSubImages.Add(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = savedMovie.Entity.Id
                        });
                    }
                   
                }
                _context.SaveChanges();
            }
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            var categories = _context.Categories.ToList();
            var cinemas = _context.Cinemas.ToList();
            var actors = _context.Actors.ToList();
            var movieSubImages = _context.MovieSubImages.ToList();
            return View(new MovieVM()
            {
                Movie = movie,
                Actors = actors,
                Categories = categories,
                Cinemas = cinemas,
                MovieSubImages = movieSubImages
            });
        }

        //Post Update
        [HttpPost]
        public IActionResult Update(Movie movie, IFormFile ImageFile, List<IFormFile> MovieSubImages)
        {
            var movieInDb = _context.Movies.AsNoTracking().FirstOrDefault(c => c.Id == movie.Id);

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
            var updateMovie = _context.Update(movie);
            _context.SaveChanges();
            //_________________________________________________________________________________
            //Images
            if (MovieSubImages != null && MovieSubImages.Count > 0)
            {
                var oldMovieSubImages = _context.MovieSubImages.Where(m => m.MovieId == movie.Id);
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
                _context.MovieSubImages.RemoveRange(oldMovieSubImages);
                _context.SaveChanges();
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
                        _context.MovieSubImages.Add(new MovieSubImage()
                        {
                            Image = fileName,
                            MovieId = movie.Id
                        });
                    }
                }
            }
            _context.SaveChanges();
            TempData["Successfully"] = "Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        //Delete SubImage
        public IActionResult DeleteSubImage(int movieId, string Image)
        {
            var imageInDb = _context.MovieSubImages.Where(m=>m.MovieId == movieId && m.Image == Image).FirstOrDefault();
            if (imageInDb != null)
            {
                var filPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", imageInDb.Image);
                if (System.IO.File.Exists(filPath))
                {
                    System.IO.File.Delete(filPath);
                }
                _context.MovieSubImages.Remove(imageInDb);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Update), new {id = movieId});
        }
        //Delete
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movie",movie.MainImage);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            // remove MovieSubImages from wwwroot
            var oldMovieSubImages = _context.MovieSubImages.Where(p => p.MovieId == movie.Id);
            foreach (var item in oldMovieSubImages)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "MovieSubImage", item.Image);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

            }
            _context.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
