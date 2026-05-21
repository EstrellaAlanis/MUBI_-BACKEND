using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Mubi.Api.Config;
using Mubi.Api.Services.Interfaces;

namespace Mubi.Api.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendVerificationCodeAsync(string toEmail, string code, string purpose)
    {
        if (string.IsNullOrWhiteSpace(_settings.SenderEmail) || string.IsNullOrWhiteSpace(_settings.Password))
            throw new Exception("La configuración de correo no está completa. Revisa EmailSettings en appsettings.json.");

        var titulo = purpose.Equals("registro", StringComparison.OrdinalIgnoreCase)
            ? "Verifica tu correo para crear tu cuenta MUBI"
            : "Código de acceso a MUBI";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = titulo;

        message.Body = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='font-family:Arial,sans-serif;background:#071008;padding:26px;color:#f4fff0;border-radius:14px'>
                    <h2 style='color:#59ff00;margin:0 0 12px'>MUBI Textil Store</h2>
                    <p>Usa este código para continuar:</p>
                    <div style='font-size:34px;font-weight:900;letter-spacing:8px;background:#101713;color:#59ff00;padding:18px;border-radius:12px;text-align:center'>
                        {code}
                    </div>
                    <p style='margin-top:16px;color:#cfe4ce'>Este código vence en 10 minutos.</p>
                    <p style='color:#8fa08f;font-size:13px'>Si no solicitaste este código, ignora este mensaje.</p>
                </div>",
            TextBody = $"Tu código MUBI es: {code}. Vence en 10 minutos."
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
