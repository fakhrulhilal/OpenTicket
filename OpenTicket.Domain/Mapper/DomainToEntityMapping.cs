using AutoMapper;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;
using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Mapper
{
    public class DomainToEntityMapping : Profile
    {
        public DomainToEntityMapping()
        {
            CreateMap<AddEmailAccountCommand, EmailAccount>()
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.UserId))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Secret));
            this.IgnoreUnmapped<AddEmailAccountCommand, EmailAccount>();
            CreateMap<EditEmailAccountCommand, EmailAccount>()
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.UserId))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Secret));
            this.IgnoreUnmapped<EditEmailAccountCommand, EmailAccount>();
        }
    }
}