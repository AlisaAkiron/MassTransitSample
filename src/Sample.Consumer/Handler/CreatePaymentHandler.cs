using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sample.Common.Contract;
using Sample.Database;
using Sample.Database.Entities;

namespace Sample.Consumer.Handler;

public class CreatePaymentHandler(SampleDbContext dbContext) : IConsumer<CreatePayment>
{
    public async Task Consume(ConsumeContext<CreatePayment> context)
    {
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == context.Message.CustomerId)
                ?? throw new InvalidOperationException("Customer not found");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = context.Message.OrderId,
            Customer = customer,
            Amount = context.Message.Amount
        };

        await dbContext.Payments.AddAsync(payment);

        await context.Publish(new PaymentCreated(payment.OrderId, payment.Id));

        await dbContext.SaveChangesAsync();
    }
}
