using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
        private readonly string[] _requiredScopes =
            {"openid", "profile", "email", "offline_access", "IMAP.AccessAsUser.All", "POP.AccessAsUser.All"};

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

        [HttpPost]
        public async Task<ActionResult> SignInExternal(EditEmailAccountCommand command)
        {
            QueryEmailAccountById.EmailAccount emailAccount;
            if (string.IsNullOrWhiteSpace(command.UserId))
            {
                emailAccount = await _mediator.Send(new QueryEmailAccountById(command.Id));
                if (emailAccount == null)
                {
                    ModelState.AddModelError(nameof(EditEmailAccountCommand.Id), "Unknown email account");
                    return RedirectToAction("Edit", new {command.Id});
                }
            }
            else
            {
                emailAccount = _mapper.Map<QueryEmailAccountById.EmailAccount>(command);
            }

            string tenant = emailAccount.Email.Split('@')[1];
            var oAuth2AuthRequest =
                OAuth2AuthRequest.Create(tenant, emailAccount.UserId,
                    new Uri($"{Request.GetBaseUrl()}{Url.Action("LandingExternal")}"), _requiredScopes);
            return View(oAuth2AuthRequest);
        }

        [HttpPost]
        public async Task<ActionResult> LandingExternal(MsalOauthResponse response)
        {
            if (string.IsNullOrWhiteSpace(response.Code))
                return Json(response, "application/json", JsonRequestBehavior.AllowGet);
            var (tenantId, clientId, verifier) = OAuth2AuthRequest.ParseState(response.State);
            if (clientId == null)
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed,
                    "Invalid session data returned after login from M365");

            var tenant = await _mediator.Send(new QueryTenantByClientId(clientId));
            if (tenant.TenantId != tenantId)
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed,
                    "Invalid tenant returned after login from M365");
            using (var httpClient = new HttpClient())
            {
                var postBody = new Dictionary<string, string>
                {
                    ["client_id"] = tenant.ClientId,
                    ["grant_type"] = "authorization_code",
                    ["scope"] = string.Join(" ", _requiredScopes),
                    ["code"] = response.Code,
                    ["redirect_uri"] = $"{Request.GetBaseUrl()}{Url.Action("LandingExternal")}",
                    ["client_secret"] = tenant.Secret,
                    ["code_verifier"] = verifier
                };
                var httpRequest =
                    new HttpRequestMessage(HttpMethod.Post, M365Helper.BuildTokenUri(tenant.TenantId))
                    {
                        Content = new FormUrlEncodedContent(postBody)
                    };
                var httpResponse = await httpClient.SendAsync(httpRequest);
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(responseString);
                return Json(tokenResponse, "application/json", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteEmailAccountCommand(id));
            return RedirectToAction("Index");
        }
    }
}