namespace cinemanic.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;
        public List<Ticket> Tickets { get; } = new();
    }
}
