using MassTransit;
using Sample.Common.Contract;
using Sample.Database;
using Sample.Database.Entities;

namespace Sample.Consumer.Handler;

public class PlaceOrderHandler(SampleDbContext dbContext) : IConsumer<PlaceOrder>
{
    public async Task Consume(ConsumeContext<PlaceOrder> context)
    {
        var product = await dbContext.Products.FindAsync(context.Message.ProductId)
                      ?? throw new InvalidOperationException("Customer or Product not found");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = context.Message.CustomerId,
            Product = product,
            Quantity = context.Message.Quantity,
            Total = product.Price * context.Message.Quantity
        };

        await dbContext.Orders.AddAsync(order);

        await context.Publish(new OrderCreated(order.Id, context.Message.CustomerId, context.Message.ProductId, order.Total));

        await dbContext.SaveChangesAsync();
    }
}
