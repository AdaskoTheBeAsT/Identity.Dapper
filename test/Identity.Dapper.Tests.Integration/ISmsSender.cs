using System.Threading.Tasks;

namespace Identity.Dapper.Tests.Integration
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
