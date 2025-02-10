using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeGrabAdminService.Dto;
using ShoeGrabCommonModels;
using static ProductManagement;

namespace ShoeGrabAdminService.Controllers;

[Route("api/admin/products")]
[Authorize(Roles = UserRole.Admin)]
public class ProductManagementController : ControllerBase
{
    private readonly ProductManagementClient _client;
    private readonly IMapper _mapper;

    public ProductManagementController(ProductManagementClient client, IMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductDto request)
    {
        var productMapped = _mapper.Map<ProductProto>(request);
        var grpcRequest = new AddProductRequest { Product = _mapper.Map<ProductProto>(request) };
        var response = await _client.AddProductAsync(grpcRequest);
        
        if (response.Success)
        {
            return Ok(request);
        }

        return BadRequest("Unable to update product");
    }

    [HttpPut]
    public async Task<ActionResult<ProductDto>> UpdateProduct([FromBody] ProductDto request)
    {
        var grpcRequest = new UpdateProductRequest { Product = _mapper.Map<ProductProto>(request) };
        var response = await _client.UpdateProductAsync(grpcRequest);

        if (response.Success)
        {
            return Ok(request);
        }

        return BadRequest("Unable to add product");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var request = new DeleteProductRequest { Id = id };
        var response = await _client.DeleteProductAsync(request);

        if (response.Success)
        {
            return Ok();
        }

        return BadRequest();
    }
}
