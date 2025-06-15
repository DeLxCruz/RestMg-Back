namespace Domain.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int DisplayOrder { get; set; }
         public bool IsActive { get; set; } = true;

        public ICollection<MenuItem> Items { get; set; } = [];
    }
}