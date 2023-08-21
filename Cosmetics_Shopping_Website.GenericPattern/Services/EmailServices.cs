using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class EmailServices: IEmailServices
    {
        //to get email configuration from appsettings.json
        private readonly EmailConfiguration _emailConfig;
        public EmailServices(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email",_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Content;

            emailMessage.Body = bodyBuilder.ToMessageBody();
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private async void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.CheckCertificateRevocation = false;
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                    
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(mailMessage);

                }
                catch
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    //client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
