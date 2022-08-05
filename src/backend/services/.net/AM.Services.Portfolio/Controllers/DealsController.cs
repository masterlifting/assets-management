using System;
using System.Threading.Tasks;
using AM.Services.Portfolio.Services.Http;
using Microsoft.AspNetCore.Mvc;
using static AM.Services.Common.Contracts.Helpers.ServiceHelper;

namespace AM.Services.Portfolio.Controllers;

[ApiController, Route("[controller]")]
public class DealsController : ControllerBase
{
    private readonly DealApi api;
    public DealsController(DealApi api) => this.api = api;

    [HttpGet]
    public async Task<IActionResult> Get(int page = 0, int limit = 0)
    {
        try
        {
            return Ok(await api.GetAsync(new Paginatior(page, limit)));
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
            return Ok(await api.GetAsync(companyId, new Paginatior(page, limit)));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}