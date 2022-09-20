using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;
using AM.Services.Portfolio.Core.Services.ControllersApi;
using Shared.Data;

namespace AM.Services.Portfolio.Host.Controllers;

[ApiController, Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventApi api;
    public EventsController(EventApi api) => this.api = api;

    [HttpGet]
    public async Task<IActionResult> Get(int page = 0, int limit = 0)
    {
        try
        {
            return Ok(await api.GetAsync(new Paginator<Event>(page, limit)));
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
            return Ok(await api.GetAsync(companyId, new Paginator<Event>(page, limit)));
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}