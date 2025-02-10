namespace ShoeGrabAdminService.Dto;
public class AdminOrderSearchQuery
{
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}
