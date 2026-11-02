using Cairo_Cinema.Data;
using Cairo_Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var cinemas = _context.Cinemas.AsQueryable();

            return View(cinemas.AsEnumerable());
        }

        //Get Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cinema());
        }

        //Post Create
        [HttpPost]
        public IActionResult Create(Cinema cinema,IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                cinema.Image = fileName;
            }

            _context.Add(cinema);
            _context.SaveChanges();
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);
            return View(cinema);
        }

        //Post Update
        [HttpPost]
        public IActionResult Update(Cinema cinema, IFormFile ImageFile)
        {
            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);
            

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                cinema.Image = fileName;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema", cinemaInDb.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);

                }
            }
            else
            {
                cinema.Image = cinemaInDb.Image;
            }

            _context.Update(cinema);
            _context.SaveChanges();
            TempData["Successfully"] = "Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema",cinema.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _context.Remove(cinema);
            _context.SaveChanges();
            TempData["Successfully"] = "Deleted Successfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
