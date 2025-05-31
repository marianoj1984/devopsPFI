using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using tecni_devops;
using tecni_devops.Controllers;
using tecni_devops.Models; // Asegurate de tener este namespace si ToDoItem está ahí
using Xunit;

namespace TecniDevOps.Tests
{
    public class ToDoItemControllerTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName) // un nombre distinto para cada test
                .Options;

            var context = new AppDbContext(options);
            return context;
        }

        [Fact]
        public void GetAll_ReturnsAllItems()
        {
            var context = GetDbContext("GetAllDB");
            context.ToDoItems.Add(new ToDoItem { Titulo = "Item 1", EstaCompleto = false });
            context.ToDoItems.Add(new ToDoItem { Titulo = "Item 2", EstaCompleto = true });
            context.SaveChanges();

            var controller = new ToDoItemController(context);

            var result = controller.GetAll() as OkObjectResult;
            var items = Assert.IsAssignableFrom<System.Collections.Generic.List<ToDoItem>>(result.Value);

            Assert.Equal(2, items.Count);
        }

        [Fact]
        public void Create_AddsItem_ReturnsCreated()
        {
            var context = GetDbContext("CreateDB");
            var controller = new ToDoItemController(context);

            var newItem = new ToDoItem { Titulo = "Nuevo item", EstaCompleto = false };
            var result = controller.Create(newItem) as CreatedAtActionResult;

            Assert.NotNull(result);
            var item = result.Value as ToDoItem;
            Assert.Equal("Nuevo item", item.Titulo);
            Assert.Single(context.ToDoItems);
        }

        [Fact]
        public void Update_ModifiesItem_ReturnsNoContent()
        {
            var context = GetDbContext("UpdateDB");
            var item = new ToDoItem { Titulo = "Viejo", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);

            var update = new ToDoItem { Titulo = "Nuevo", EstaCompleto = true };
            var result = controller.Update(item.Id, update) as NoContentResult;

            Assert.NotNull(result);
            var updatedItem = context.ToDoItems.Find(item.Id);
            Assert.Equal("Nuevo", updatedItem.Titulo);
            Assert.True(updatedItem.EstaCompleto);
        }

        [Fact]
        public void Update_NonExistentItem_ReturnsNotFound()
        {
            var context = GetDbContext("UpdateFailDB");
            var controller = new ToDoItemController(context);

            var update = new ToDoItem { Titulo = "Algo", EstaCompleto = true };
            var result = controller.Update(999, update);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_RemovesItem_ReturnsNoContent()
        {
            var context = GetDbContext("DeleteDB");
            var item = new ToDoItem { Titulo = "Para borrar", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var result = controller.Delete(item.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.ToDoItems);
        }

        [Fact]
        public void Delete_NonExistentItem_ReturnsNotFound()
        {
            var context = GetDbContext("DeleteFailDB");
            var controller = new ToDoItemController(context);

            var result = controller.Delete(123);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
