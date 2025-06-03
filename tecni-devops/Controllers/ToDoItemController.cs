using Microsoft.AspNetCore.Mvc;
using System.Linq;
using tecni_devops.Models;

namespace tecni_devops.Controllers
{
    public class ToDoItemController : Controller
    {
        private readonly AppDbContext _context;

        public ToDoItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ToDoItem
        public IActionResult Index()
        {
            var items = _context.ToDoItems.ToList();
            return View(items);
        }

        // GET: /ToDoItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ToDoItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ToDoItem item)
        {
            if (ModelState.IsValid)
            {
                _context.ToDoItems.Add(item);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: /ToDoItem/Update/{id}
        public IActionResult Update(int id)
        {
            var item = _context.ToDoItems.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /ToDoItem/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ToDoItem item)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.ToDoItems.Find(item.Id);
                if (existing == null) return NotFound();

                existing.Titulo = item.Titulo;
                existing.EstaCompleto = item.EstaCompleto;
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: /ToDoItem/Delete/{id}
        public IActionResult Delete(int id)
        {
            var item = _context.ToDoItems.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /ToDoItem/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.ToDoItems.Find(id);
            if (item == null) return NotFound();

            _context.ToDoItems.Remove(item);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
