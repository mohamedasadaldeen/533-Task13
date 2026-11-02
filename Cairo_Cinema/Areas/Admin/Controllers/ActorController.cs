using Cairo_Cinema.Data;
using Cairo_Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var actors = _context.Actors.AsQueryable();

            return View(actors.AsEnumerable());
        }

        //Get Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }

        //Post Create
        [HttpPost]
        public IActionResult Create(Actor actor,IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if(ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                actor.Image = fileName;
            }

            _context.Add(actor);
            _context.SaveChanges();
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            return View(actor);
        }

        //Post Update
        [HttpPost]
        public IActionResult Update(Actor actor, IFormFile ImageFile)
        {
            var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                actor.Image = fileName;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", actorInDb.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);

                }
            }
            else
            {
                actor.Image = actorInDb.Image;
            }

            _context.Update(actor);
            _context.SaveChanges();
            TempData["Successfully"] = "Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", actor.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _context.Remove(actor);
            _context.SaveChanges();
            TempData["Successfully"] = "Deleted Successfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
