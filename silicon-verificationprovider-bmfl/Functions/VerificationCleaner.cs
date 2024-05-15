using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using silicon_verificationprovider_bmfl.Data.Contexts;
using silicon_verificationprovider_bmfl.Services;

namespace silicon_verificationprovider_bmfl.Functions
{
    public class VerificationCleaner(ILogger<VerificationCleaner> logger, IVerificationCleanerService verificationCleanerService)
    {
        private readonly ILogger<VerificationCleaner> _logger = logger;
        private readonly IVerificationCleanerService _verificationCleanerService = verificationCleanerService;

        [Function("VerificationCleaner")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            try
            {
                await _verificationCleanerService.RemoveExpiredRecordsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : VerificationCleaner.Run() :: {ex.Message}");
            }
        }
    }
}
