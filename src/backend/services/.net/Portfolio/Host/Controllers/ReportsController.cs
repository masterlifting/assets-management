using AM.Services.Portfolio.Core.Abstractions.Persistense.Repositories;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Core.Constants.Persistense.Enums;
using static Shared.Persistense.Constants.Catalogs;

using PortfolioCoreException = AM.Services.Portfolio.Host.Exceptions.PortfolioHostException;

namespace AM.Services.Portfolio.Host.Controllers
{
    [ApiController, Route("[controller]")]
    public sealed class ReportsController : ControllerBase
    {
        private readonly IReportDataRepository _reportDataRepository;
        private readonly IUserRepository _userRepository;

        private static readonly Dictionary<string, int> ProviderPatterns = new()
        {
            { "^B_k-(.+)_ALL(.+).xls$", (int)Providers.Bcs }
        };
        public ReportsController(
            IReportDataRepository reportDataRepository
            , IUserRepository userRepository)
        {
            _reportDataRepository = reportDataRepository;
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

                    var id = Convert.ToBase64String(SHA256.HashData(payload));

                    var createdResult = await _reportDataRepository.TryCreateAsync(new()
                    {
                        Id = id,
                        UserId = userId,
                        ProviderId = GetProviderId(file.FileName),
                        Name = file.FileName,
                        Source = nameof(ReportsController),
                        ContentTypeId = (int)ContentTypeDictionary[file.ContentType],
                        Payload = payload
                    });

                    if (!createdResult.IsSuccess)
                        throw new PortfolioCoreException(nameof(ReportsController), $"Сохранение файла отчета: {file.FileName}", createdResult.Error!);
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
}