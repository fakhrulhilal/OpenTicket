using FluentValidation;
using Newtonsoft.Json;

namespace OpenTicket.Web.Models
{
    public class MsalOauthResponse
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string Error { get; set; }
        [JsonProperty("error_description")]
        public string Error_Description { get; set; }

        public class Validator : AbstractValidator<MsalOauthResponse>
        { }
    }
}