using System.ComponentModel.DataAnnotations;

namespace ShoeGrabAdminService.Dto;

public class ProductDto
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public double Price { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string ImageUrl { get; set; }
}
