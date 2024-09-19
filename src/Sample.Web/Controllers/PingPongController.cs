using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Contract.Models;
using Sample.Database;

namespace Sample.Web.Controllers;

[ApiController]
[AutoConstructor]
public partial class PingPongController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly SampleDbContext _dbContext;

    [HttpGet("ping")]
    public async Task<IActionResult> PingAsync()
    {
        var connectionId = Request.HttpContext.Connection.Id;
        await _publishEndpoint.Publish(new PingReceivedEvent(connectionId));
        await _dbContext.SaveChangesAsync();

        return Ok($"pong! your request id:{connectionId}");
    }
}
