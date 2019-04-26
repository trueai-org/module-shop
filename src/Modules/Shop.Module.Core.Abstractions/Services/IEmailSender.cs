using System.Threading.Tasks;


namespace Shop.Module.Core.Abstractions.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, bool isHtml = false);
    }
}
