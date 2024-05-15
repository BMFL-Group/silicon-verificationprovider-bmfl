
namespace silicon_verificationprovider_bmfl.Services
{
    public interface IVerificationCleanerService
    {
        Task RemoveExpiredRecordsAsync();
    }
}