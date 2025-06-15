namespace Domain.Models
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}