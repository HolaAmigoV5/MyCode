namespace BlazorWebassemblyApisite
{
    public class StudentRepository : IStudentRepository
    {
        private static List<Student> _students = new()
        {
            new Student{Id=1,Name="小红", Age=10, Class="1班", Sex="女" },
            new Student{Id=2,Name="小明", Age=12, Class="2班", Sex="男" },
            new Student{Id=3,Name="小强", Age=12, Class="3班", Sex="男" }
        };

        public bool Add(Student student)
        {
            _students.Add(student);
            return true;
        }

        public bool Delete(int id)
        {
            var stu = _students.FirstOrDefault(s => s.Id == id);
            if (stu != null)
            {
                _students.Remove(stu);
            }
            return true;
        }

        public Student Get(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id) ?? new Student();
        }

        public List<Student> List()
        {
            return _students;
        }

        public bool Update(Student student)
        {
            var stu = _students.FirstOrDefault(s => s.Id == student.Id);
            if (stu != null)
            {
                _students.Remove(stu);
            }
            _students.Add(student);
            return true;
        }
    }
}
