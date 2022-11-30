using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;
using Shared.Persistence.Abstractions.Repositories;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Shared.Persistence.Abstractions.Constants.Enums;

using PortfolioCoreException = AM.Services.Portfolio.API.Exceptions.PortfolioHostException;

namespace AM.Services.Portfolio.API.Controllers;

[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly Guid _userId = Guid.Parse("0f9075e9-bbcf-4eef-a52d-d9dcad816f5e");

    private readonly ILogger<ReportsController> _logger;
    private readonly IMongoDBRepository _mongoRepository;
    private readonly IPostgreSQLRepository _postgreSQLRepository;
    private static readonly Dictionary<string, Core.Constants.Enums.Providers> ProviderPatterns = new()
    {
        { "^B_k-(.+)_ALL(.+).xls$", Core.Constants.Enums.Providers.Bcs }
    };

    public ReportsController(ILogger<ReportsController> logger, IMongoDBRepository mongoRepository, IPostgreSQLRepository postgreSQLRepository)
    {
        _logger = logger;
        _mongoRepository = mongoRepository;
        _postgreSQLRepository = postgreSQLRepository;
    }

    [HttpPost("{stepId}")]
    public async Task<IActionResult> Post(int stepId, IFormFileCollection files)
    {
        try
        {
            await CreateUserAsync();

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length));

                if (GetProvider(file.FileName) != Core.Constants.Enums.Providers.Bcs)
                    _logger.LogError(new PortfolioCoreException(nameof(ReportsController), $"File: '{file.FileName}'", new("The provider is notn BCS")));

                var reportData = new IncomingData
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,

                    Payload = payload,
                    PayloadHash = SHA256.HashData(payload),
                    PayloadHashAlgoritm = nameof(SHA256),
                    
                    PayloadSource = file.FileName,
                    PayloadContentType = file.ContentType,
                    
                    ProcessStepId = stepId,
                    ProcessStatusId = (int)ProcessStatuses.Ready
                };

                var createdResult = await _mongoRepository.TryCreateAsync(reportData);

                if (!createdResult.IsSuccess)
                    _logger.LogError(new PortfolioCoreException(nameof(ReportsController), $"File: '{file.FileName}'", new(createdResult.Error!)));
            }

            return Ok();
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static Core.Constants.Enums.Providers GetProvider(string fileName)
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
        var user = await _postgreSQLRepository.FindAsync<User>(_userId);

        if (user is null)
            await _postgreSQLRepository.CreateAsync(new User
            {
                Id = _userId,
                Name = "Andrey Pestunov"
            });
    }
}