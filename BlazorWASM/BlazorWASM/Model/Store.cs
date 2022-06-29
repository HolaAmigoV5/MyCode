namespace BlazorWASM.Model
{
    public class Store
    {
        private List<Student>? _students;

        public void SetStudents(List<Student> list)
        {
            _students = list;
        }

        public List<Student> GetStudents() => _students ?? new List<Student>();

        public Student GetStudentById(int id)
        {
            var stu = _students?.FirstOrDefault(s => s.Id == id) ?? new Student();
            return stu;
        }
    }
}
