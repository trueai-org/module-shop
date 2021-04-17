using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace Shop.Module.EmailSenderSmtp
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSenderSmtpOptions _emailConfig = new EmailSenderSmtpOptions();
        private readonly IRepository<EmailSend> _emilSendRepository;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IConfiguration config,
            IRepository<EmailSend> emilSendRepository,
            ILogger<EmailSender> logger,
            IAppSettingService appSettingService)
        {
            _emailConfig = appSettingService.Get<EmailSenderSmtpOptions>().Result;
            _emilSendRepository = emilSendRepository;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string body, bool isHtml = false)
        {
            var send = new EmailSend();
            try
            {
                if (string.IsNullOrWhiteSpace(_emailConfig.SmtpUserName))
                    throw new ArgumentNullException(nameof(_emailConfig.SmtpUserName));
                if (string.IsNullOrWhiteSpace(_emailConfig.SmtpPassword))
                    throw new ArgumentNullException(nameof(_emailConfig.SmtpPassword));

                send.From = _emailConfig.SmtpUserName;
                send.To = email;
                send.Subject = subject;
                send.IsHtml = isHtml;
                send.Body = body;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.SmtpUserName));
                message.To.Add(new MailboxAddress(email));
                message.Subject = subject;

                var textFormat = isHtml ? TextFormat.Html : TextFormat.Plain;
                message.Body = new TextPart(textFormat)
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {
                    // Accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(_emailConfig.SmtpHost, _emailConfig.SmtpPort, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_emailConfig.SmtpUserName, _emailConfig.SmtpPassword);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                send.IsSucceed = true;
                _logger.LogInformation("邮件发送成功", send);
            }
            catch (Exception ex)
            {
                send.Message = ex.Message;
                send.IsSucceed = false;
                _logger.LogError(ex, "邮件发送异常", send, email, subject, body, isHtml);
            }
            finally
            {
                _emilSendRepository.Add(send);
                await _emilSendRepository.SaveChangesAsync();
            }
        }
    }
}
