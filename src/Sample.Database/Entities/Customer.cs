namespace Sample.Database.Entities;

public record Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Balance { get; set; }
}
