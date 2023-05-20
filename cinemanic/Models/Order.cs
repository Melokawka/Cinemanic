namespace cinemanic.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int AccountId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public Account Account { get; set; } = null!;
        public List<Ticket> Tickets { get; } = new();
    }

    public enum OrderStatus
    {
        PENDING,
        SUBMITTED,
        PAID,
        COMPLETED
    }
}
