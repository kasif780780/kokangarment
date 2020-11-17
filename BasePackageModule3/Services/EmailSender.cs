using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace BasePackageModule3.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }
        public EmailSender(IOptions<EmailOptions> emailoptions)
        {
            Options = emailoptions.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, email, message);
        }

        private Task Execute(string sendGridKey, string subject, string email, string message)
        {
            var client = new SendGridClient(sendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("kasif780780@gmail.com", "Base Package"),
                Subject = subject,
                PlainTextContent = message,

            };
            msg.AddTo(new EmailAddress(email));
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        

    }
}
