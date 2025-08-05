//using System.Data.Entity;
using System.Globalization;
using EventCalendar.Context;
using EventCalendar.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventCalendar.Controllers
{
    public class EventController : Controller
    {
        private readonly EventContext _context;

        public EventController(EventContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult EventList()
        {
            var events = _context.Events.Include(e=>e.Category).ToList();
            return View(events);
        }

        public JsonResult GetEvents()
        {
            var events = _context.Events.Include(e => e.Category).ToList();
            var result = events.Select(e => new
            {
                id = e.EventId,
                title = e.Title,
                start = e.StartDate.ToString("s"),
                end = e.EndDate.ToString("s"),
                categoryId = e.CategoryId,
                color = e.Category != null ? e.Category.Color : "#ee4141"
            });
            return Json(result);
        }

        public JsonResult GetLastEvents()
        {
            var events = _context.Events.Include(e=>e.Category).OrderByDescending(e => e.EventId).Take(5).ToList();
            var result = events.Select(e => new
            {
                id = e.EventId,
                title = e.Title,
                start = e.StartDate.ToString("s"),
                end = e.EndDate.ToString("s"),
                categoryId = e.CategoryId,
                color = e.Category != null ? e.Category.Color : "#3788d8"
            });
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddEvent(string title, DateTime start, DateTime end, int categoryId)
        {
            var result = new Event
            {
                Title = title,
                StartDate = start,
                EndDate = end,
                CategoryId = categoryId
            };
            _context.Events.Add(result);
            _context.SaveChanges();
            return Json(new { success = true, eventId = result.EventId });
        }

        public ActionResult UpdateEventDate(int id)
        {
            var result=_context.Events.Find(id);
            if (result == null) 
            {
                return NotFound();
            }
            ViewBag.Categories=_context.Categories.ToList();
            return View(result);
        }
        //takvim sürükle bırak
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateEventDate(int id, string start, string end)
        {
            var result = _context.Events.Find(id);
            if (result == null)
            {
                return Json(new { success = false, message = "Event Bulunamadı" });
            }
            DateTime startDate, endDate;
            if(!DateTime.TryParse(start, out startDate) || !DateTime.TryParse(end, out endDate))
            {
                return Json(new {success=false, message= "Geçersiz tarih formatı." });
            }
            result.StartDate = startDate;
            result.EndDate = endDate;
            _context.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteEvent(int id)
        {
            var result = _context.Events.Find(id);
            if (result == null)
            {
                return Json(new {success=false,message = "Event Bulunamadı" });
            }
            _context.Events.Remove(result);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
