using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _cinemaRepository;// = new Repository<Cinema>();

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaRepository.GetListAsync();

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
        public async Task<IActionResult> Create(Cinema cinema,IFormFile ImageFile)
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

            await _cinemaRepository.AddAsync(cinema);
            await _cinemaRepository.CommitAsync();
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            return View(cinema);
        }

        //Post Update
        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema, IFormFile ImageFile)
        {
            var cinemaInDb = await _cinemaRepository.GetOneAsync(c => c.Id == cinema.Id,isTracked:false);
            

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

            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            TempData["Successfully"] = "Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema",cinema.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _cinemaRepository.Remove(cinema);
            await _cinemaRepository.CommitAsync();
            TempData["Successfully"] = "Deleted Successfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
