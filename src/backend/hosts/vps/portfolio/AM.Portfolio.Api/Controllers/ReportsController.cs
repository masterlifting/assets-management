using System.Threading.Tasks;

using AM.Portfolio.Api.Abstractions.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AM.Portfolio.Api.Controllers;

//TODO: Add versioning
[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportsService _service;
    public ReportsController(IReportsService service) => _service = service;

    [HttpPost("{stepId}")]
    public async Task<IActionResult> Post(int stepId, IFormFileCollection files)
    {
        //TODO: UserId should be taken from the token
        var result = await _service.Post(1, stepId, files, default);
        
        //TODO: Add a common response logic with a model and different filters
        return Ok($"Processed files count: {result.Data.Length};\nUnprocessed files count: {result.Errors.Length};");
    }
}
