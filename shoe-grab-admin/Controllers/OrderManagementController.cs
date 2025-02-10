using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeGrabCommonModels;
using static OrderManagement;
using ShoeGrabAdminService.Dto;
using Google.Protobuf.WellKnownTypes;
using AutoMapper;

namespace ShoeGrabAdminService.Controllers;

[Route("api/admin/order")]
[Authorize(Roles = UserRole.Admin)]
public class OrderManagementController : ControllerBase
{
    private readonly OrderManagementClient _client;
    private readonly IMapper _mapper;

    public OrderManagementController(OrderManagementClient client, IMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var response = await _client.DeleteOrderAsync(new DeleteOrderRequest { Id = orderId });
        if (!response.Success)
            return BadRequest("Something went wrong during order delete operation.");

        return Ok();
    }

    [HttpPatch]
    [Route("{orderId}/status")]
    public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromBody] OrderStatusUpdateDto request)
    {
        var response = await _client.ChangeOrderStatusAsync(new ChangeOrderStatusRequest { Id = orderId, Status = request.NewStatus });
        if (!response.Success)
            return BadRequest("Something went wrong during order status update.");

        return Ok();
    }

    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> SearchOrders(
        [FromQuery] AdminOrderSearchQuery query)
    {
        var request = ConstructRequest(query);
        var response = await _client.SearchOrdersAsync(request);
        var ordersMapped = response.Orders.Select(o => _mapper.Map<OrderDto>(o)).ToList();

        return ordersMapped;
    }

    private SearchOrdersRequest ConstructRequest(AdminOrderSearchQuery query)
    {
        var request = new SearchOrdersRequest();
        request.UserId = query.UserId.HasValue ? query.UserId.Value : -1;
        request.StartDate = query.StartDate.HasValue ?
            Timestamp.FromDateTime(query.StartDate.Value.ToUniversalTime()) : 
            Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime());
        request.EndDate = query.EndDate.HasValue ?
            Timestamp.FromDateTime(query.EndDate.Value.ToUniversalTime()) :
            Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime());
        request.Status = !string.IsNullOrEmpty(query.Status) ?
            query.Status :
            string.Empty;

        return request;
    }
}