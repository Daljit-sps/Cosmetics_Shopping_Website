using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;
using MimeKit;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IEmailServices
    {
        void SendEmail(Message message);
    }
}
