using ASPNETCore_DB.Models;

namespace ASPNETCore_DB.Interfaces
{
    public interface IStudent
    {
        IQueryable<Student> GetAll();
        Student GetById(string id);
        Student ByEmail(string email);
        Student Create(Student student);
        Student Edit(Student student);
        bool Delete(string id);
        bool IsExist(string id);
    }
}