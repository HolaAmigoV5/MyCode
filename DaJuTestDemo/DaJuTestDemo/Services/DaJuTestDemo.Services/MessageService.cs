using DaJuTestDemo.Services.Interfaces;

namespace DaJuTestDemo.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
