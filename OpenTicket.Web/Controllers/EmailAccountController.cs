using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using OpenTicket.Domain.Command;
using OpenTicket.Web.Models;

namespace OpenTicket.Web.Controllers
{
    public class EmailAccountController : Controller
    {
        private readonly MediatR.IMediator _mediator;
        private readonly AutoMapper.IMapper _mapper;

        public EmailAccountController(MediatR.IMediator mediator, AutoMapper.IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ActionResult> Index(PagedRequest parameter)
        {
            var result = await _mediator.Send(new QueryEmailAccounts
            {
                CurrentPage = parameter.Page ?? 1,
                PageSize = parameter.Size ?? 20
            });
            return View(result);
        }

        public ActionResult Create() => View();

        [HttpPost]
        public async Task<ActionResult> Create(AddEmailAccountCommand request)
        {
            await _mediator.Send(request);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var emailAccount = await _mediator.Send(new QueryEmailAccountById(id));
            var command = _mapper.Map<EditEmailAccountCommand>(emailAccount);
            return View(command);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditEmailAccountCommand request)
        {
            await _mediator.Send(request);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteEmailAccountCommand(id));
            return RedirectToAction("Index");
        }
    }
}