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
public class SplitsController : DateControllerBase<Split, SplitPostDto, SplitGetDto>
{
    public SplitsController(RestApiWrite<Split, SplitPostDto> apiWrite, RestApiRead<Split, SplitGetDto> apiRead)
        : base(apiWrite, apiRead) { }
}