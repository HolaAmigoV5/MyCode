namespace BlazorWebassemblyApisite
{
    public interface IStudentRepository
    {
        List<Student> List();
        Student Get(int id);
        bool Add(Student student);
        bool Update(Student student);
        bool Delete(int id);
    }
}
