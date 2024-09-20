namespace Sample.Common.Contract;

public record PlaceOrder(int CustomerId, int ProductId, int Quantity);

public record OrderCreated(Guid OrderId, int CustomerId, int ProductId, decimal Bill);
