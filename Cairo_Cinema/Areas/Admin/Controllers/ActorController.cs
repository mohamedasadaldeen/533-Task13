using Microsoft.AspNetCore.Mvc;

namespace Cairo_Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Actor> _actorRepository;// = new Repository<Actor>();

        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _actorRepository.GetListAsync();

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
        public async Task<IActionResult> Create(Actor actor, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                actor.Image = fileName;
            }

            await _actorRepository.AddAsync(actor);
            await _actorRepository.CommitAsync();
            TempData["Successfully"] = "Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        //Get Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);
            return View(actor);
        }

        //Post Update
        [HttpPost]
        public async Task<IActionResult> Update(Actor actor, IFormFile ImageFile)
        {
            var actorInDb = await _actorRepository.GetOneAsync(c => c.Id == actor.Id, isTracked: false);

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

            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync();
            TempData["Successfully"] = "Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor", actor.Image);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);

            }
            _actorRepository.Remove(actor);
            await _actorRepository.CommitAsync();
            TempData["Successfully"] = "Deleted Successfully";
            return RedirectToAction(nameof(Index));

        }
    }
}
