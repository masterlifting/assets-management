using AM.Services.Portfolio.API.Services;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Microsoft.AspNetCore.Mvc;

using Shared.Models.Pagination;

using System;
using System.Threading.Tasks;

namespace AM.Services.Portfolio.API.Controllers;

[ApiController, Route("[controller]")]
public sealed class DealsController : ControllerBase
{
    private readonly DealApi api;
    public DealsController(DealApi api) => this.api = api;

    [HttpGet]
    public async Task<IActionResult> Get(int page = 0, int limit = 0)
    {
        try
        {
            return Ok(await api.GetAsync(new Paginator<Deal>(page, limit)));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
    [HttpGet("{companyId}")]
    public async Task<IActionResult> Get(string companyId, int page = 0, int limit = 0)
    {
        try
        {
            return Ok(await api.GetAsync(companyId, new Paginator<Deal>(page, limit)));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}