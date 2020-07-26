using System.Threading.Tasks;

namespace Identity.Dapper.Samples.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
