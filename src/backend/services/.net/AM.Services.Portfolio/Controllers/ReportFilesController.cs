using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Shared.Core.Exceptions;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Controllers;

[ApiController, Route("[controller]")]
public class ReportFilesController : ControllerBase
{
    private readonly DatabaseContext _context;

    private static readonly Dictionary<string, int> ProviderPatterns = new()
    {
        { "^B_k-(.+)_ALL(.+).xls$", (int)Providers.BCS }
    };
    public ReportFilesController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFileCollection files)
    {
        try
        {
            const string userId = "0f9075e9-bbcf-4eef-a52d-d9dcad816f5e";

            if (await _context.Users.FindAsync(userId) is null)
                await _context.Users.AddAsync(new()
                {
                    Id = userId,
                    Name = "Пестунов Андрей"
                });

            var reportFiles = new List<ReportFile>(files.Count);

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload, 0, (int)file.Length);

                reportFiles.Add(new()
                {
                    UserId = userId,
                    ProviderId = GetProviderId(file.FileName),
                    Name = file.FileName,
                    ContentType = file.ContentType,
                    Payload = payload
                });
            }

            await _context.ReportFiles.AddRangeAsync(reportFiles);

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

        throw new SharedCoreNotCastException("ReportFileController", "Определение провайдера отчета", $"Не удалось у файла {fileName}");
    }
}