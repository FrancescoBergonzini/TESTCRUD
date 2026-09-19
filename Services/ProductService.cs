using System.Collections.Concurrent;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}

public interface IProductService
{
    PagedResult<Product> GetAll(string? search, int page, int pageSize);
    Product? GetById(int id);
    Product Create(CreateProductRequest request);
    Product? Update(int id, UpdateProductRequest request);
    bool Delete(int id);
}

public sealed class ProductService : IProductService
{
    private readonly ConcurrentDictionary<int, Product> products = new();
    private int nextId;

    public PagedResult<Product> GetAll(string? search, int page, int pageSize)
    {
        var query = products.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product =>
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                product.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
        }

        var ordered = query.OrderBy(product => product.Id).ToList();
        var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
        return new PagedResult<Product>(items, page, pageSize, ordered.Count);
    }

    public Product? GetById(int id) => products.GetValueOrDefault(id);

    public Product Create(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Interlocked.Increment(ref nextId),
            Name = request.Name!.Trim(),
            Price = decimal.Round(request.Price, 2),
            Description = request.Description?.Trim()
        };

        products[product.Id] = product;
        return product;
    }

    public Product? Update(int id, UpdateProductRequest request)
    {
        if (!products.ContainsKey(id))
        {
            return null;
        }

        var updated = new Product
        {
            Id = id,
            Name = request.Name!.Trim(),
            Price = decimal.Round(request.Price, 2),
            Description = request.Description?.Trim()
        };

        products[id] = updated;
        return updated;
    }

    public bool Delete(int id) => products.TryRemove(id, out _);
}