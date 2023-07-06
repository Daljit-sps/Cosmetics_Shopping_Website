using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}
