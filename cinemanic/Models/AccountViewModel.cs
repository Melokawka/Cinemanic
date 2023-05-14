namespace cinemanic.Models
{
    public class AccountViewModel
    {
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<ArchivedTicket> ArchivedTickets { get; set; }
        public List<Order> Orders { get; set; }
        public List<Like> Likes { get; set; }


    }

}
