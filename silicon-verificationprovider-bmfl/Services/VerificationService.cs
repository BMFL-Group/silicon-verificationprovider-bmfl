using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using silicon_verificationprovider_bmfl.Data.Contexts;
using silicon_verificationprovider_bmfl.Models;

namespace silicon_verificationprovider_bmfl.Services;

public class VerificationService(IServiceProvider serviceProvider, ILogger<VerificationService> logger) : IVerificationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<VerificationService> _logger = logger;

    public VerificationRequestModel UnpackVerificationRequest(ServiceBusReceivedMessage message)
    {
        try
        {
            if (message != null)
            {
                var verificationRequest = JsonConvert.DeserializeObject<VerificationRequestModel>(message.Body.ToString());
                if (verificationRequest != null && !string.IsNullOrEmpty(verificationRequest.Email))
                {
                    return verificationRequest;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.UnpackVerificationRequest() :: {ex.Message}");
        }
        return null!;
    }
    public string GenerateCode()
    {
        try
        {
            var rnd = new Random();
            var code = rnd.Next(100000, 999999);

            return code.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GenerateCode() :: {ex.Message}");
        }
        return null!;
    }

    public EmailRequestModel GenerateEmailRequest(VerificationRequestModel verificationRequest, string code)
    {
        try
        {
            if (!string.IsNullOrEmpty(verificationRequest.Email) && !string.IsNullOrEmpty(code))
            {
                var emailRequest = new EmailRequestModel()
                {
                    To = verificationRequest.Email,
                    Subject = $"VerificationCode {code}",
                    HtmlBody = $@"
                        
                            <html Lang='en'>
                                <head>
                                <meta charset='UTF-8'>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                <title>Verifivcation Code</title>
                                </head>
                                <body>
                                    <div>
                                        <p>Dear User,</p>
                                        <p>Here is your verification code for {verificationRequest.Email}: {code}</p>
                                        <p>If you did not request this code please ignore this mail. Should you receive multiple requests that you did not ask for then contact us.</p>
                                    </div>

                                </body>
                            </html>
                       ",
                    PlainText = $"Here is your verification code for {verificationRequest.Email}: {code}"

                };

                return emailRequest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GenerateEmailRequest() :: {ex.Message}");
        }
        return null!;
    }

    public async Task<bool> SaveVerificationRequest(VerificationRequestModel verificationRequest, string code)
    {
        try
        {
            using var context = _serviceProvider.GetRequiredService<DataContext>();

            var existingRequest = await context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
            if (existingRequest != null)
            {
                existingRequest.Code = code;
                existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                context.Entry(existingRequest).State = EntityState.Modified;
            }
            else
            {
                context.VerificationRequests.Add(new Data.Entities.VerificationRequestEntity()
                {
                    Email = verificationRequest.Email,
                    Code = code
                });
            }

            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.SaveVerificationRequest() :: {ex.Message}");
        }
        return false;
    }

    public string GenerateServiceBusEmailRequest(EmailRequestModel emailRequest)
    {
        try
        {
            var payload = JsonConvert.SerializeObject(emailRequest);
            if (!string.IsNullOrEmpty(payload))
            {
                return payload;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : VerificationService.GenerateServiceBusEmailRequest() :: {ex.Message}");
        }
        return null!;
    }
}
