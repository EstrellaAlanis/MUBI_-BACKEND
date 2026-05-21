namespace Mubi.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string toEmail, string code, string purpose);
}
