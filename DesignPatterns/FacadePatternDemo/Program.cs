using System;

namespace FacadePatternDemo
{
    /// <summary>
    /// 以学生选课系统为例演示外观模式的使用
    /// 学生选课模块包括功能有：验证选课的人数是否已满，通知用户课程选择成功与否
    /// </summary>
    class Program
    {
        private static readonly RegistrationFacade facade = new RegistrationFacade();
        static void Main(string[] args)
        {
            if (facade.RegisterCourse("设计模式", "Learning Hard"))
                Console.WriteLine("选修成功");
            else
                Console.WriteLine("选课失败");

            Console.ReadLine();
        }
    }

    /// <summary>
    /// 外观类
    /// </summary>
    public class RegistrationFacade
    {
        private RegisterCourse registerCourse;
        private NotifyStudent notifyStudent;
        public RegistrationFacade()
        {
            registerCourse = new RegisterCourse();
            notifyStudent = new NotifyStudent();
        }

        public bool RegisterCourse(string courseName, string studentName)
        {
            if (!registerCourse.CheckAvailable(courseName))
                return false;
            return notifyStudent.Notify(studentName);
        }
    }

    #region 子系统
    /// <summary>
    /// 相当于子系统A
    /// </summary>
    public class RegisterCourse
    {
        public bool CheckAvailable(string courseName)
        {
            Console.WriteLine("正在验证课程{0}是否人数已满", courseName);
            return true;
        }
    }

    /// <summary>
    /// 相当于子系统B
    /// </summary>
    public class NotifyStudent
    {
        public bool Notify(string studentName)
        {
            Console.WriteLine("正在向{0}发送通知", studentName);
            return true;
        }
    } 
    #endregion
}
