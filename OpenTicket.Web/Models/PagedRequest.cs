using System.ComponentModel;
using FluentValidation;

namespace OpenTicket.Web.Models
{
    public class PagedRequest
    {
        [DefaultValue(1)]
        public int? Page { get; set; }

        [DefaultValue(20)]
        public int? Size { get; set; }

        public class PagedRequestValidator : AbstractValidator<PagedRequest>
        { }
    }
}