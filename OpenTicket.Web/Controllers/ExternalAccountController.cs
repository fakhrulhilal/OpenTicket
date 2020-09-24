using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Web.Controllers
{
    public class ExternalAccountController : Controller
    {
        private readonly IMediator _mediator;

        public ExternalAccountController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult> Index(MailProtocolType? protocol)
        {
            var allExternalAccounts = await _mediator.Send(new GetAllExternalsAccountQuery
            {
                Protocol = protocol
            });
            if (Request.IsAjaxRequest())
                return Json(allExternalAccounts, JsonRequestBehavior.AllowGet);
            return View(allExternalAccounts);
        }

        [HttpGet]
        public ActionResult Create() => View();

        [HttpPost]
        public async Task<ActionResult> Create(AddExternalAccountCommand command)
        {
            await _mediator.Send(command);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var externalAccount = await _mediator.Send(new GetExternalAccountDetailQuery(id));
            return View(externalAccount);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditExternalAccountCommand command)
        {
            await _mediator.Send(command);
            return RedirectToAction("Index");
        }
    }
}