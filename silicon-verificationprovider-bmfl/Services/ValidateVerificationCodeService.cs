﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using silicon_verificationprovider_bmfl.Data.Contexts;
using silicon_verificationprovider_bmfl.Models;

namespace silicon_verificationprovider_bmfl.Services;

public class ValidateVerificationCodeService(ILogger<ValidateVerificationCodeService> logger, DataContext context) : IValidateVerificationCodeService
{
    private readonly ILogger<ValidateVerificationCodeService> _logger = logger;
    private readonly DataContext _context = context;

    public async Task<ValidateRequest> UnPackValidateRequestAsync(HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var validaterequest = JsonConvert.DeserializeObject<ValidateRequest>(body);
                if (validaterequest != null)
                {
                    return validaterequest;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : ValidateVerificationCode.UnPackValidateRequest() :: {ex.Message}");
        }

        return null!;
    }

    public async Task<bool> ValidateCodeAsync(ValidateRequest validateRequest)
    {
        try
        {
            var entity = await _context.VerificationRequests.FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);
            if (entity != null)
            {
                _context.VerificationRequests.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : ValidateVerificationCode.ValidateCodeAsync() :: {ex.Message}");
        };
        return false;
    }
}