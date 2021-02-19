using ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace ef
{
    public class CoursesDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=.\sqlexpress;initial catalog=Courses;Integrated Security=yes");
        }
    }
}