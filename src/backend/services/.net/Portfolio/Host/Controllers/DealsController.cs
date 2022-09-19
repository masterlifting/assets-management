﻿using System;
using System.Threading.Tasks;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Services.ControllersApi;

using Microsoft.AspNetCore.Mvc;

using Shared.Data;

namespace AM.Services.Portfolio.Host.Controllers;

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