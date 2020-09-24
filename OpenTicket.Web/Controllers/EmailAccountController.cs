using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using OpenTicket.Domain.Command;
using OpenTicket.Helper;
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
            var emailAccount = await _mediator.Send(new GetEmailAccountByIdQuery(id));
            var command = _mapper.Map<EditEmailAccountCommand>(emailAccount);
            return View(command);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditEmailAccountCommand request)
        {
            await _mediator.Send(request);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> SignInExternal(SaveTemporaryEmailAccountCommand command)
        {
            await _mediator.Send(command);
            var externalAccount = await _mediator.Send(new GetExternalAccountDetailQuery(command.ExternalAccountId ?? default));
            var oAuth2AuthRequest = OAuth2Helper.CreateRequest(command.Email, externalAccount.Identifier, externalAccount.ClientId,
                $"{Request.GetBaseUrl()}{Url.Action("LandingExternal")}", M365Helper.EmailScopes);
            return View(oAuth2AuthRequest);
        }

        [HttpPost]
        public async Task<ActionResult> LandingExternal(MsalOauthResponse response)
        {
            if (string.IsNullOrWhiteSpace(response.Code))
                return Json(response, "application/json", JsonRequestBehavior.AllowGet);
            var state = OAuth2Helper.ParseState(response.State);
            if (state.ClientId == null)
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed,
                    "Invalid session data returned after login from M365");

            var tenant = await _mediator.Send(new GetTenantByIdentifierQuery(state.Tenant));
            if (tenant.ClientId != state.ClientId)
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed,
                    "Invalid tenant returned after login from M365");
            var tokenResponse = await OAuth2Helper.AcquireTokenAsync(new AcquireTokenRequest
            {
                Tenant = tenant.Identifier,
                ClientId = tenant.ClientId,
                Secret = tenant.Secret,
                ResponseCode = response.Code,
                CodeVerifier = state.CodeVerifier,
                RedirectUri = $"{Request.GetBaseUrl()}{Url.Action("LandingExternal")}"
            });
            await _mediator.Send(new SaveEmailAccountTokenCommand
            {
                Email = state.Email,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            });
            return View(tokenResponse);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteEmailAccountCommand(id));
            return RedirectToAction("Index");
        }
    }
}