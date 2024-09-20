namespace Sample.Common.Contract;

public record CreatePayment(Guid OrderId, int CustomerId, decimal Amount);

public record PaymentCreated(Guid OrderId, Guid PaymentId);

public record MakePayment(Guid OrderId, Guid PaymentId);

public record PaymentDone(Guid OrderId, Guid PaymentId);

public record PaymentFailed(Guid OrderId, Guid PaymentId, string Reason);
