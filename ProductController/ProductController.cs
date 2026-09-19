using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public sealed class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public ActionResult<PagedResult<Product>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return BadRequest(new { message = "page deve essere >= 1 e pageSize deve essere tra 1 e 100." });
        }

        return Ok(productService.GetAll(search, page, pageSize));
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = productService.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create(CreateProductRequest request)
    {
        var product = productService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Product> Update(int id, UpdateProductRequest request)
    {
        var result = productService.Update(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return productService.Delete(id) ? NoContent() : NotFound();
    }
}