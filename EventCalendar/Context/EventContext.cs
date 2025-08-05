using EventCalendar.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventCalendar.Context
{
    public class EventContext:DbContext
    {
        public EventContext(DbContextOptions<EventContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}
