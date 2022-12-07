using AM.Services.Portfolio.API.Exceptions;
using AM.Services.Portfolio.Core.Domain.Persistence.Collections;
using AM.Services.Portfolio.Core.Domain.Persistence.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shared.Persistence.Abstractions.Repositories;

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

using static Shared.Persistence.Abstractions.Constants.Enums;

namespace AM.Services.Portfolio.API.Controllers;

[ApiController, Route("[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly Guid _userId = Guid.Parse("0f9075e9-bbcf-4eef-a52d-d9dcad816f5e");

    private readonly ILogger<ReportsController> _logger;
    private readonly IMongoDBRepository _mongoRepository;
    private readonly IPostgreSQLRepository _postgreSQLRepository;

    public ReportsController(ILogger<ReportsController> logger, IMongoDBRepository mongoRepository, IPostgreSQLRepository postgreSQLRepository)
    {
        _logger = logger;
        _mongoRepository = mongoRepository;
        _postgreSQLRepository = postgreSQLRepository;
    }

    [HttpPost("{stepId}")]
    public async Task<IActionResult> Post(int stepId, IFormFileCollection files)
    {
        try
        {
            var user = await _postgreSQLRepository.FindAsync<User>(_userId);

            if (user is null)
                await _postgreSQLRepository.CreateAsync(new User
                {
                    Id = _userId,
                    Name = "Andrey Pestunov"
                });

            foreach (var file in files)
            {
                var payload = new byte[file.Length];
                await using var stream = file.OpenReadStream();
                var _ = await stream.ReadAsync(payload.AsMemory(0, (int)file.Length));

                var createdResult = await _mongoRepository.TryCreateAsync(new IncomingData
                {
                    Id = Guid.NewGuid(),
                    UserId = user!.Id,

                    Payload = payload,
                    PayloadHash = SHA256.HashData(payload),
                    PayloadHashAlgoritm = nameof(SHA256),
                    PayloadSource = file.FileName,
                    PayloadContentType = file.ContentType,

                    ProcessStepId = stepId,
                    ProcessStatusId = (int)ProcessStatuses.Ready
                });

                if (!createdResult.IsSuccess)
                    throw new PortfolioAPIException(nameof(ReportsController), $"Saving file: '{file.FileName}'", new(createdResult.Error!));
            }

            return Ok($"{files.Count} elements were created");
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
}