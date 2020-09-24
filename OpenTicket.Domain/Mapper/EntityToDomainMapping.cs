﻿using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Mapper
{
    public class EntityToDomainMapping : AutoMapper.Profile
    {
        public EntityToDomainMapping()
        {
            CreateMap<Data.Entity.EmailAccount, Command.QueryEmailAccounts.Result>()
                .ForMember(dst => dst.Tenant, map => map.MapFrom(src => src.ExternalAccount != null ? src.ExternalAccount.Identifier : null))
                .ForMember(dst => dst.ClientId, map => map.MapFrom(src => src.ExternalAccount != null ? src.ExternalAccount.ClientId : null))
                .ForMember(dst => dst.Secret, map => map.MapFrom(src => src.ExternalAccount != null ? src.ExternalAccount.Secret : null));
            this.IgnoreUnmapped<Data.Entity.EmailAccount, Command.QueryEmailAccounts.Result>();
            CreateMap<Data.Entity.EmailAccount, Command.QueryEmailAccountById.EmailAccount>();
            this.IgnoreUnmapped<Data.Entity.EmailAccount, Command.QueryEmailAccountById.EmailAccount>();
            CreateMap<Data.Entity.Ticket, Command.QueryTickets.Ticket>()
                .ForMember(dst => dst.CustomerName, map => map.MapFrom(src => src.Customer.DisplayName))
                .ForMember(dst => dst.CustomerEmail, map => map.MapFrom(src => src.Customer.Email));
            this.IgnoreUnmapped<Data.Entity.Ticket, Command.QueryTickets.Ticket>();
            CreateMap<Data.Entity.ExternalAccount, Command.GetTenantByIdentifierQuery.Result>();
            this.IgnoreUnmapped<Data.Entity.ExternalAccount, Command.GetTenantByIdentifierQuery.Result>();
            CreateMap<Data.Entity.ExternalAccount, Command.GetAllExternalAccountQuery.Result>();
            this.IgnoreUnmapped<Data.Entity.ExternalAccount, Command.GetAllExternalAccountQuery.Result>();
            CreateMap<Data.Entity.ExternalAccount, Command.GetExternalAccountDetailQuery.Result>();
            this.IgnoreUnmapped<Data.Entity.ExternalAccount, Command.GetExternalAccountDetailQuery.Result>();
        }
    }
}
