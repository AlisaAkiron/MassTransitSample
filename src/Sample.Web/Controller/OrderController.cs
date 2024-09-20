using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Sample.Common.Contract;

namespace Sample.Web.Controller;

[ApiController]
[AutoConstructor]
public partial class OrderController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    [HttpPost("order")]
    public async Task<IActionResult> MakeOrderAsync([FromBody] MakeOrderRequest request)
    {
        await _publishEndpoint.Publish(new PlaceOrder(request.CustomerId, request.ProductId, request.Quantity));

        return Accepted();
    }
}

public sealed record MakeOrderRequest(int CustomerId, int ProductId, int Quantity);
