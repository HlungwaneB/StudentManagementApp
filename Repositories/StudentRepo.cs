using ASPNETCore_DB.Data;
using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;

namespace ASPNETCore_DB.Repositories
{
    public class StudentRepo : IStudent
    {
        private readonly SQLiteDBContext _context;

        public StudentRepo(SQLiteDBContext context)
        {
            _context = context;
        }

        public IQueryable<Student> GetAll()
        {
            return _context.Students;
        }

        public Student GetById(string id)
        {
            var student = _context.Students?.FirstOrDefault(x => x.StudentNumber == id);
            return student;
        }

        public Student ByEmail(string email)
        {
            var student = _context.Students?.FirstOrDefault(x => x.Email == email);
            return student;
        }

        public Student Create(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return student;
        }

        public Student Edit(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return student;
        }

        public bool Delete(string id)
        {
            var student = _context.Students?.FirstOrDefault(x => x.StudentNumber == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool IsExist(string id)
        {
            return _context.Students.Any(x => x.StudentNumber == id);
        }
    }
}