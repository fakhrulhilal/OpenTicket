using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Mapper
{
    public class DomainToEntityMapping : AutoMapper.Profile
    {
        public DomainToEntityMapping()
        {
            CreateMap<Command.AddEmailAccountCommand, Data.Entity.EmailAccount>()
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.UserId))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Secret));
            this.IgnoreUnmapped<Command.AddEmailAccountCommand, Data.Entity.EmailAccount>();
        }
    }
}