using AM.Services.Portfolio.Core.Domain.Persistense.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Persistense.Abstractions.Repositories;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Abstractions.Constants.Enums;

using PortfolioCoreException = AM.Services.Portfolio.API.Exceptions.PortfolioHostException;

namespace AM.Services.Portfolio.API.Controllers;

[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly Guid _userId = Guid.Parse("0f9075e9-bbcf-4eef-a52d-d9dcad816f5e");

    private readonly ILogger<ReportsController> _logger;
    private readonly IPostgreSQLRepository _repository;

    private static readonly Dictionary<string, Providers> ProviderPatterns = new()
{
    { "^B_k-(.+)_ALL(.+).xls$", Providers.Bcs }
};

    public ReportsController(ILogger<ReportsController> logger, IPostgreSQLRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost("bcs")]
    public async Task<IActionResult> CreateBcsReport(IFormFileCollection files)
    {
        try
        {
            await CreateUserAsync();

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length));

                if (GetProvider(file.FileName) == Providers.Bcs)
                    _logger.LogError(new PortfolioCoreException("BCS" + nameof(ReportsController), $"Saving report: '{file.FileName}'", new("The provider is notn BCS")));

                var reportData = new DataAsBytes
                {
                    UserId = _userId,
                    Payload = payload,
                    SHA256Hash = SHA256.HashData(payload),
                    PayloadSource = file.FileName,
                    PayloadContentType = file.ContentType,
                    ProcessStepId = (int)ProcessSteps.ParseBcsReportDataToJson,
                    ProcessStatusId = (int)ProcessableEntityStatuses.Ready
                };

                var createdResult = await _repository.TryCreateAsync(reportData);

                if (!createdResult.IsSuccess)
                    _logger.LogError(new PortfolioCoreException("BCS" + nameof(ReportsController), $"Saving report: '{file.FileName}'", new(createdResult.Error!)));
            }

            return Ok();
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static Providers GetProvider(string fileName)
    {
        foreach (var (pattern, provider) in ProviderPatterns)
        {
            var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
                return provider;
        }

        throw new PortfolioCoreException(nameof(ReportsController), nameof(GetProvider), new($"Report provider not recognized for '{fileName}'"));
    }
    private async Task CreateUserAsync()
    {
        var user = await _repository.FindAsync<User>(_userId);

        if (user is null)
            await _repository.CreateAsync(new User
            {
                Id = _userId,
                Name = "Andrey Pestunov"
            });
    }
}