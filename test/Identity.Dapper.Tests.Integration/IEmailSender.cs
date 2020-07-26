using System.Threading.Tasks;

namespace Identity.Dapper.Tests.Integration
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
