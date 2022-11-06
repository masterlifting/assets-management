using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants.Catalogs;

using PortfolioCoreException = AM.Services.Portfolio.Host.Exceptions.PortfolioHostException;

namespace AM.Services.Portfolio.Host.Controllers;

[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private const string UserId = "0f9075e9-bbcf-4eef-a52d-d9dcad816f5e";
    
    private readonly ILogger<ReportsController> _logger;
    private readonly IReportDataRepository _reportDataRepository;
    private readonly IUserRepository _userRepository;

    private static readonly Dictionary<string, int> ProviderPatterns = new()
    {
        { "^B_k-(.+)_ALL(.+).xls$", (int)Providers.Bcs }
    };
    public ReportsController(
        ILogger<ReportsController> logger
        , IReportDataRepository reportDataRepository
        , IUserRepository userRepository)
    {
        _logger = logger;
        _reportDataRepository = reportDataRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFileCollection files)
    {
        try
        {
            await CreateUserAsync(UserId);

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length));

                var reportData = new ReportData
                {
                    Id = Convert.ToBase64String(SHA256.HashData(payload)),
                    UserId = UserId,
                    ProviderId = GetProviderId(file.FileName),
                    Name = file.FileName,
                    Source = nameof(ReportsController),
                    ContentTypeId = (int)ContentTypeDictionary[file.ContentType],
                    Payload = payload
                };

                var createdResult = await _reportDataRepository.TryCreateAsync(reportData);

                if (!createdResult.IsSuccess)
                    _logger.LogError(new PortfolioCoreException(nameof(ReportsController), $"Saving report: '{file.FileName}'", createdResult.Error!));
            }

            return Ok();
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static int GetProviderId(string fileName)
    {
        foreach (var (pattern, providerId) in ProviderPatterns)
        {
            var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
                return providerId;
        }

        throw new PortfolioCoreException(nameof(ReportsController), nameof(GetProviderId), $"Report provider not recognized for '{fileName}'");
    }
    private async Task CreateUserAsync(string userId)
    {
        var user = await _userRepository.FindAsync(userId);

        if (user is null)
            await _userRepository.CreateAsync(new()
            {
                Id = userId,
                Name = "Andrey Pestunov"
            });
    }
}