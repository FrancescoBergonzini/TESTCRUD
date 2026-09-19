using System.ComponentModel.DataAnnotations;

public sealed class Product
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public string? Description { get; init; }
}

public sealed class CreateProductRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string? Name { get; init; }

    [Range(0.01, 1000000000)]
    public decimal Price { get; init; }

    [StringLength(1000)]
    public string? Description { get; init; }
}

public sealed class UpdateProductRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string? Name { get; init; }

    [Range(0.01, 1000000000)]
    public decimal Price { get; init; }

    [StringLength(1000)]
    public string? Description { get; init; }
}