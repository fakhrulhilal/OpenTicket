using System;

namespace OpenTicket.Data.Entity
{
    public class Ticket
    {
        public int Id { get; set; }
        public int? EmailAccountId { get; set; }
        public int CustomerId { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual EmailAccount EmailAccount { get; set; }
    }
}
