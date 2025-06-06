using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Index_ReturnsAllItems()
        {
            var context = GetDbContext("IndexDB");
            context.ToDoItems.AddRange(
                new ToDoItem { Titulo = "Uno", EstaCompleto = false },
                new ToDoItem { Titulo = "Dos", EstaCompleto = true }
            );
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var result = controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<ToDoItem>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var controller = new ToDoItemController(GetDbContext("CreateGetDB"));
            var result = controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Post_ValidItem_RedirectsToIndex()
        {
            var context = GetDbContext("CreatePostValidDB");
            var controller = new ToDoItemController(context);
            var item = new ToDoItem { Titulo = "Nuevo", EstaCompleto = false };

            var result = controller.Create(item) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Single(context.ToDoItems);
        }

        [Fact]
        public void Create_Post_InvalidModel_ReturnsViewWithItem()
        {
            var context = GetDbContext("CreatePostInvalidDB");
            var controller = new ToDoItemController(context);
            controller.ModelState.AddModelError("Titulo", "Required");

            var item = new ToDoItem();
            var result = controller.Create(item) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(item, result.Model);
        }

        [Fact]
        public void Update_Get_ExistingItem_ReturnsViewWithItem()
        {
            var context = GetDbContext("UpdateGetExistsDB");
            var item = new ToDoItem { Titulo = "Edit", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var result = controller.Update(item.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(item.Id, ((ToDoItem)result.Model).Id);
        }

        [Fact]
        public void Update_Get_NonExistingItem_ReturnsNotFound()
        {
            var controller = new ToDoItemController(GetDbContext("UpdateGetNotFoundDB"));
            var result = controller.Update(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_Post_ExistingItem_RedirectsToIndex()
        {
            var context = GetDbContext("UpdatePostExistsDB");
            var item = new ToDoItem { Titulo = "Viejo", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var updated = new ToDoItem { Id = item.Id, Titulo = "Nuevo", EstaCompleto = true };

            var result = controller.Update(updated) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            var dbItem = context.ToDoItems.Find(item.Id);
            Assert.Equal("Nuevo", dbItem.Titulo);
            Assert.True(dbItem.EstaCompleto);
        }

        [Fact]
        public void Update_Post_NonExistingItem_ReturnsNotFound()
        {
            var controller = new ToDoItemController(GetDbContext("UpdatePostNotFoundDB"));
            var item = new ToDoItem { Id = 999, Titulo = "Algo", EstaCompleto = true };
            var result = controller.Update(item);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_Post_InvalidModel_ReturnsViewWithItem()
        {
            var controller = new ToDoItemController(GetDbContext("UpdatePostInvalidDB"));
            controller.ModelState.AddModelError("Titulo", "Required");

            var item = new ToDoItem();
            var result = controller.Update(item) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(item, result.Model);
        }

        [Fact]
        public void Delete_Get_ExistingItem_ReturnsViewWithItem()
        {
            var context = GetDbContext("DeleteGetExistsDB");
            var item = new ToDoItem { Titulo = "Borrar", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var result = controller.Delete(item.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(item.Id, ((ToDoItem)result.Model).Id);
        }

        [Fact]
        public void Delete_Get_NonExistingItem_ReturnsNotFound()
        {
            var controller = new ToDoItemController(GetDbContext("DeleteGetNotFoundDB"));
            var result = controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_ExistingItem_RedirectsToIndex()
        {
            var context = GetDbContext("DeleteConfirmedExistsDB");
            var item = new ToDoItem { Titulo = "Borrar", EstaCompleto = false };
            context.ToDoItems.Add(item);
            context.SaveChanges();

            var controller = new ToDoItemController(context);
            var result = controller.DeleteConfirmed(item.Id) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Null(context.ToDoItems.Find(item.Id));
        }

        [Fact]
        public void DeleteConfirmed_NonExistingItem_ReturnsNotFound()
        {
            var controller = new ToDoItemController(GetDbContext("DeleteConfirmedNotFoundDB"));
            var result = controller.DeleteConfirmed(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }


}
