namespace CRNProductAPI.Models
{
    public class Item
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation property
        public Product? Product { get; set; }
    }
}
