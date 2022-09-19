using AM.Services.Portfolio.Core.Domain.Persistense.Entities.Enums;
using Shared.Extensions.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;
using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Host.Controllers;

[ApiController, Route("[controller]")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger;
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    private static readonly Dictionary<string, int> ProviderPatterns = new()
    {
        { "^B_k-(.+)_ALL(.+).xls$", (int)Providers.Bcs }
    };
    public ReportController(
        ILogger<ReportController> logger
        , IReportRepository reportRepository
        , IUserRepository userRepository)
    {
        _logger = logger;
        _reportRepository = reportRepository;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFileCollection files)
    {
        try
        {
            const string userId = "0f9075e9-bbcf-4eef-a52d-d9dcad816f5e";
            await CreateUserAsync(userId);

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload, 0, (int)file.Length);
                
                try
                {
                    await _reportRepository.CreateAsync(new()
                    {
                        UserId = userId,
                        ProviderId = GetProviderId(file.FileName),
                        Name = file.FileName,
                        ContentType = file.ContentType,
                        Payload = payload
                    });
                }
                catch (Exception exception)
                {
                    _logger.LogError($"{nameof(ReportController)}.{nameof(Post)}",$"Сохранение {file.FileName}", exception);
                }
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

        throw new PortfolioCoreException("ReportFileController", "Определение провайдера отчета", $"Не удалось у файла {fileName}");
    }
    private async Task CreateUserAsync(string userId)
    {
        if (await _userRepository.DbSet.FindAsync(userId) is null)
            await _userRepository.CreateAsync(new()
            {
                Id = userId,
                Name = "Пестунов Андрей"
            });
    }
}