using System.Security.AccessControl;
using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Mapper
{
    public class EntityToDomainMapping : AutoMapper.Profile
    {
        public EntityToDomainMapping()
        {
            CreateMap<Data.Entity.EmailAccount, Command.QueryEmailAccounts.EmailAccount>()
                .ForMember(dst => dst.UserId, map => map.MapFrom(src => src.Username))
                .ForMember(dst => dst.Secret, map => map.MapFrom(src => src.Password));
            this.IgnoreUnmapped<Data.Entity.EmailAccount, Command.QueryEmailAccounts.EmailAccount>();
            CreateMap<Data.Entity.EmailAccount, Command.QueryEmailAccountById.EmailAccount>()
                .ForMember(dst => dst.UserId, map => map.MapFrom(src => src.Username))
                .ForMember(dst => dst.Secret, map => map.MapFrom(src => src.Password));
            this.IgnoreUnmapped<Data.Entity.EmailAccount, Command.QueryEmailAccountById.EmailAccount>();
            CreateMap<Data.Entity.Ticket, Command.QueryTickets.Ticket>()
                .ForMember(dst => dst.CustomerName, map => map.MapFrom(src => src.Customer.DisplayName))
                .ForMember(dst => dst.CustomerEmail, map => map.MapFrom(src => src.Customer.Email));
            this.IgnoreUnmapped<Data.Entity.Ticket, Command.QueryTickets.Ticket>();
        }
    }
}
