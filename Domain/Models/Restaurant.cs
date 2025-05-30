namespace Domain.Models
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? BrandingColor { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Table> Tables { get; set; } = new List<Table>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}