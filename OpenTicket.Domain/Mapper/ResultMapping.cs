using OpenTicket.Domain.Command;
using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Mapper
{
    public class ResultMapping : AutoMapper.Profile
    {
        public ResultMapping()
        {
            CreateMap<QueryEmailAccountById.EmailAccount, EditEmailAccountCommand>();
            CreateMap<MailClient.IMailMessage, CreateTicketCommand>()
                .ForMember(p => p.CustomerEmail, map => map.MapFrom(src => src.From.Address))
                .ForMember(p => p.CustomerName, map => map.MapFrom(src => src.From.DisplayName))
                .ForMember(p => p.Title, map => map.MapFrom(src => src.Subject))
                .ForMember(p => p.Question, map => map.MapFrom(src => src.Body));
            this.IgnoreUnmapped<MailClient.IMailMessage, CreateTicketCommand>();
            CreateMap<EditEmailAccountCommand, SaveTemporaryEmailAccountCommand>();
            this.IgnoreUnmapped<EditEmailAccountCommand, SaveTemporaryEmailAccountCommand>();
        }
    }
}
