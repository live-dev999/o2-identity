using System.Threading.Tasks;

namespace O2.Identity.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
