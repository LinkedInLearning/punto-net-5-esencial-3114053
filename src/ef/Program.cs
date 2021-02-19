using System;
using System.Collections.Generic;

namespace ef
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new CoursesDbContext();
            dbContext.Database.EnsureCreated();

            dbContext.Authors.Add(new Entities.Author(){
                Name = "Rodrigo Díaz Concha",
                Courses = new List<Entities.Course>(new [] {
                    new Entities.Course(){ Name = ".NET 5 esencial" },
                    new Entities.Course(){ Name = ".NET Core esencial" },
                    new Entities.Course(){ Name = ".NET Core avanzado" },
                    new Entities.Course(){ Name = ".NET Entity Framework Core esencial" }
                })
            });

            dbContext.SaveChanges();

            System.Console.WriteLine("¡Listo!");
        }
    }
}
