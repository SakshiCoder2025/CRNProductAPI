namespace CRNProductAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
    }
}
