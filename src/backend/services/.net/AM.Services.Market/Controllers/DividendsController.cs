using AM.Services.Market.Controllers.Base;
using AM.Services.Market.Domain.Entities;
using AM.Services.Market.Models.Api.Http;
using AM.Services.Market.Services.Http.Common;
using Microsoft.AspNetCore.Mvc;

namespace AM.Services.Market.Controllers;

[ApiController]
[Route("companies/[controller]")]
[Route("companies/{companyId}/[controller]")]
[Route("companies/{companyId}/sources/{sourceId:int}/[controller]")]
public class DividendsController : DateControllerBase<Dividend, DividendPostDto, DividendGetDto>
{
    public DividendsController(RestApiWrite<Dividend, DividendPostDto> apiWrite, RestApiRead<Dividend, DividendGetDto> apiRead)
        : base(apiWrite, apiRead) { }
}