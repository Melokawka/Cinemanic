namespace cinemanic.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserEmail { get; set; }
        public Account Account { get; set; }
    }
}
