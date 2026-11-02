using Cairo_Cinema.Data;
using Cairo_Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var categories = _context.Categories.AsQueryable();

            return View(categories.AsEnumerable());
        }

        //Get Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post Create
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            return View(category);
        }

        //Post Update
        [HttpPost]
        public IActionResult Update(Category category)
        {
            
            _context.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            _context.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}
