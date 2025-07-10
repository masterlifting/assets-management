using AM.Services.Portfolio.API.Exceptions;
using AM.Services.Portfolio.API.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shared.Extensions.Logging;

using System;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.API.Controllers;

[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly ILogger<ReportsController> _logger;
    private IReportApi _reportApi;
    public ReportsController(ILogger<ReportsController> logger, IReportApi reportApi)
    {
        _logger = logger;
        _reportApi = reportApi;
    }

    [HttpPost("{stepId}")]
    public async Task<IActionResult> Post(int stepId, IFormFileCollection files)
    {
        var userId = Guid.Parse("0f9075e9-bbcf-4eef-a52d-d9dcad816f5e");

        var result = await _reportApi.TrySaveFilesAsync(userId, stepId, files);

        if (!result.IsSuccess)
        {
            _logger.LogError(new PortfolioAPIException(nameof(ReportsController), nameof(Post), new(string.Join(";", result.Errors))));
            return BadRequest("Sorry, some went wrong.");
        }

        return Ok($"Files count: {result.Data!.Length} was saved to database");
    }
}