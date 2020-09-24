using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using OpenTicket.Domain.Command;

namespace OpenTicket.Web.Controllers
{
    public class TicketController : Controller
    {
        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<ActionResult> Index()
        {
            var tickets = (await _mediator.Send(new GetAllTicketsQuery())).ToArray();
            return View(tickets);
        }
    }
}
