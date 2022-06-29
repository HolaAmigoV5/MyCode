using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebassemblyApisite.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private IStudentRepository studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        [HttpGet]
        public List<Student> Get()
        {
            return studentRepository.List();
        }

        [HttpGet("{id}")]
        public Student Get(int id)
        {
            return studentRepository.Get(id);
        }

        [HttpPost]
        public Student Post(Student model)
        {
            studentRepository.Add(model);
            return model;
        }

        [HttpPut]
        public Student Put(Student model)
        {
            studentRepository.Update(model);
            return model;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            studentRepository.Delete(id);
        }
    }
}
