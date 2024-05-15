using Microsoft.AspNetCore.Http;
using silicon_verificationprovider_bmfl.Models;

namespace silicon_verificationprovider_bmfl.Services
{
    public interface IValidateVerificationCodeService
    {
        Task<ValidateRequest> UnPackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequest validateRequest);
    }
}