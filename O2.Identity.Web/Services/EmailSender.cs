using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace O2.Identity.Web.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager
        
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }
        
        public Task Execute(string apiKey, string subject, string message, string email)
        {
        
                    // m_log.DebugFormat("sending to='{0}', subj='{1}', msg=!!!'{2}'!!!", to, subject, bodyHtml);
                    // Debug.Assert(!string.IsNullOrEmpty(to));
                    // Debug.Assert(!string.IsNullOrEmpty(subject));
                    // Debug.Assert(!string.IsNullOrEmpty(bodyHtml));
                    //
                    // using (var client = new SmtpClient())
                    // {
                    //     client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    //     client.Host = "smtp.sendgrid.net";
                    //     client.Port = 25;//m_smtpServerPort;
                    //
                    //     using (var messageMul = new MailMessage("support@o2bionics.com",email))
                    //     {
                    //         messageMul.Subject = subject;
                    //         messageMul.IsBodyHtml = true;
                    //         messageMul.Body = message;
                    //         await client.SendMailAsync(messageMul).ConfigureAwait(false);
                    //     }
                    //     
                    // }
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("support@o2bionics.com", Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            
            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            
            return client.SendEmailAsync(msg);
        }
        // public Task SendEmailAsync(string email, string subject, string message)
        // {
        //     return Task.CompletedTask;
        // }
    }
}
