using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sample.Common.Contract;
using Sample.Database;

namespace Sample.Consumer.Handler;

public class MakePaymentHandler(SampleDbContext dbContext) : IConsumer<MakePayment>
{
    public async Task Consume(ConsumeContext<MakePayment> context)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == context.Message.PaymentId)
                      ?? throw new InvalidOperationException("Payment not found");

        if (payment.Amount > payment.Customer.Balance)
        {
            payment.Remarks = "Insufficient balance";
            await context.Publish(new PaymentFailed(payment.OrderId, context.Message.PaymentId, "Insufficient balance"));
            return;
        }
        payment.Paid = true;

        await context.Publish(new PaymentDone(payment.OrderId, payment.Id));

        await dbContext.SaveChangesAsync();
    }
}
