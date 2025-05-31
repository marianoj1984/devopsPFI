using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.ToDoItems.ToList());

        [HttpPost]
        public IActionResult Create(ToDoItem item)
        {
            _context.ToDoItems.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ToDoItem item)
        {
            var existing = _context.ToDoItems.Find(id);
            if (existing == null) return NotFound();

            existing.Titulo = item.Titulo;
            existing.EstaCompleto = item.EstaCompleto;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _context.ToDoItems.Find(id);
            if (item == null) return NotFound();

            _context.ToDoItems.Remove(item);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
