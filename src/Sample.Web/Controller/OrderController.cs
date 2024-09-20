using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.Common.Contract;
using Sample.Database;

namespace Sample.Web.Controller;

[ApiController]
[AutoConstructor]
public partial class OrderController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly SampleDbContext _sampleDbContext;

    [HttpPost("order")]
    public async Task<IActionResult> MakeOrderAsync([FromBody] MakeOrderRequest request)
    {
        await _publishEndpoint.Publish(new PlaceOrder(request.CustomerId, request.ProductId, request.Quantity));
        await _sampleDbContext.SaveChangesAsync();

        return Accepted();
    }

    [HttpGet("order")]
    public async Task<IActionResult> GetOrdersAsync()
    {
        var order = await _sampleDbContext.Orders.ToListAsync();
        return Ok(order);
    }

    [HttpGet("payment")]
    public async Task<IActionResult> GetPaymentsAsync()
    {
        var order = await _sampleDbContext.Payments.ToListAsync();
        return Ok(order);
    }

    [HttpGet("customer")]
    public async Task<IActionResult> GetCustomersAsync()
    {
        var order = await _sampleDbContext.Customers.ToListAsync();
        return Ok(order);
    }

    [HttpGet("product")]
    public async Task<IActionResult> GetProductsAsync()
    {
        var order = await _sampleDbContext.Products.ToListAsync();
        return Ok(order);
    }
}

public sealed record MakeOrderRequest(int CustomerId, int ProductId, int Quantity);
