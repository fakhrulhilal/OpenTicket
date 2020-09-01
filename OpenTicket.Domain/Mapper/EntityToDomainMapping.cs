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
        }
    }
}
