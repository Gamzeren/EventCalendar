using EventCalendar.Context;
using EventCalendar.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventCalendar.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EventContext _context;

        public CategoryController(EventContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories=_context.Categories.ToList();
            return View(categories);
        }

        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(string Name, String Color)
        {
            if(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Color))
            {
                ModelState.AddModelError("", "Name and color are required.");
                return View();
            }
            _context.Categories.Add(new Category { Name = Name, Color = Color });
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetCategories()
        {
            var category=_context.Categories.ToList();
            var result = category.Select(c => new
            {
                id = c.CategoryId,
                name = c.Name,
                color = c.Color
            });
            return Json(result);
        }

        private JsonResult Json(IEnumerable<object> result, object allowGet)
        {
            throw new NotImplementedException();
        }

        public IActionResult UpdateCategory(int id)
        {
            var value=_context.Categories.Find(id);
            if (value == null)
            {
                return NotFound();
            }
            return View(value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(EventCalendar.Entities.Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var category = _context.Categories.Find(model.CategoryId);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = model.Name;
            category.Color = model.Color;
            _context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            var category=_context.Categories.Find(id);
            if (category == null)
            {
                return Json(new {success=false, message= "Kategori bulunamadı." });
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
