using Cairo_Cinema.Data;
using Cairo_Cinema.Models;
using Cairo_Cinema.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cairo_Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;// = new ApplicationDbContext();

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(FilterVM filter)
        {
            var movies = _context.Movies.AsQueryable();
            movies = movies.Include(m => m.Category).Include(m => m.Actor);
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

            return View(movies.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
