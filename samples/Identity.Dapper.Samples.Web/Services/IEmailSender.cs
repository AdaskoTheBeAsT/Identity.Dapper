using System.Threading.Tasks;

namespace Identity.Dapper.Samples.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
