using Azure.Messaging.ServiceBus;
using silicon_verificationprovider_bmfl.Models;

namespace silicon_verificationprovider_bmfl.Services
{
    public interface IVerificationService
    {
        string GenerateCode();
        EmailRequestModel GenerateEmailRequest(VerificationRequestModel verificationRequest, string code);
        string GenerateServiceBusEmailRequest(EmailRequestModel emailRequest);
        Task<bool> SaveVerificationRequest(VerificationRequestModel verificationRequest, string code);
        VerificationRequestModel UnpackVerificationRequest(ServiceBusReceivedMessage message);
    }
}