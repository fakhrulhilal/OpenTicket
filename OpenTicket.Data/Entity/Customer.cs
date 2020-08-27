using System;
using System.Collections.Generic;

namespace OpenTicket.Data.Entity
{
    public class Customer
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredDateTime { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
