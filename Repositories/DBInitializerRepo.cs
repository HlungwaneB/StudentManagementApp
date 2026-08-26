using ASPNETCore_DB.Data;
using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;

namespace ASPNETCore_DB.Repositories
{
    public class DBInitializerRepo : IDBInitializer
    {
        public void Initialize(SQLiteDBContext context)
        {
            context.Database.EnsureCreated();

            if (context.Students.Any())
            {
                return;
            }

            var students = new Student[]
            {
                new Student
                {
                    StudentNumber = "2021000001",
                    FirstName = "Alexander",
                    Surname = "May",
                    EnrollmentDate = DateTime.Parse("2021-02-03"),
                    Photo = "DefaultPic.PNG",
                    Email = "DefaultEmail@gmail.com"
                },
                new Student
                {
                    StudentNumber = "2012000002",
                    FirstName = "Meredith",
                    Surname = "Alonso",
                    EnrollmentDate = DateTime.Parse("2021-02-01"),
                    Photo = "DefaultPic.PNG",
                    Email = "DefaultEmail@gmail.com"
                },
                new Student
                {
                    StudentNumber = "2021000003",
                    FirstName = "Arturo",
                    Surname = "Anand",
                    EnrollmentDate = DateTime.Parse("2021-02-04"),
                    Photo = "DefaultPic.PNG",
                    Email = "DefaultEmail@gmail.com"
                }
            };

            context.Students.AddRange(students);
            context.SaveChanges();
        }
    }
}