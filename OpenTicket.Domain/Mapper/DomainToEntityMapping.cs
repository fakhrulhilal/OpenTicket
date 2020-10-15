﻿using AutoMapper;
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
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.Username))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Password));
            this.IgnoreUnmapped<AddEmailAccountCommand, EmailAccount>();
            CreateMap<EditEmailAccountCommand, EmailAccount>()
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.Username))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Password));
            this.IgnoreUnmapped<EditEmailAccountCommand, EmailAccount>();
            CreateMap<SaveTemporaryEmailAccountCommand, EmailAccount>()
                .ForMember(e => e.Username, map => map.MapFrom(cmd => cmd.Username))
                .ForMember(e => e.Password, map => map.MapFrom(cmd => cmd.Password))
                .ForMember(e => e.DraftId, map => map.MapFrom(cmd => cmd.Id > 0 ? cmd.Id : AddEmailAccountCommand.DraftId))
                .ForMember(e => e.Id, map => map.MapFrom(_ => 0));
            this.IgnoreUnmapped<SaveTemporaryEmailAccountCommand, EmailAccount>();
            CreateMap<AddExternalAccountCommand, ExternalAccount>();
            this.IgnoreUnmapped<AddExternalAccountCommand, ExternalAccount>();
            CreateMap<EditExternalAccountCommand, ExternalAccount>();
            this.IgnoreUnmapped<EditExternalAccountCommand, ExternalAccount>();
        }
    }
}