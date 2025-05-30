namespace Domain.Models
{
    public class Table
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}