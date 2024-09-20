namespace Sample.Database.Entities;

public record Order
{
    public Guid Id { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Total { get; set; }

    public Product Product { get; set; } = null!;

    public string Remarks { get; set; } = string.Empty;

    public bool Cancelled { get; set; }
}
