namespace Sample.Database.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public decimal Amount { get; set; }

    public bool Paid { get; set; }

    public string Remarks { get; set; } = string.Empty;
}
