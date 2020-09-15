// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;

namespace middlerApp.API.IDP.Mappers
{
    /// <summary>
    /// Defines entity/model mapping for identity resources.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="IdentityResourceMapperProfile"/>
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            //CreateMap<Storage.Entities.IdentityResourceProperty, KeyValuePair<string, string>>()
            //    .ReverseMap();

            CreateMap<Storage.Entities.Scope, IdentityServer4.Models.IdentityResource>(MemberList.Destination)
                .ForMember(dest => dest.UserClaims, opts => opts.MapFrom(x => x.UserClaims.Select(u => u.Type)))
                .ConstructUsing(src => new IdentityServer4.Models.IdentityResource())
                .ReverseMap();

            //CreateMap<Storage.Entities.IdentityResourceClaim, string>()
            //   .ConstructUsing(x => x.Type)
            //   .ReverseMap()
            //   .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<Storage.Entities.Scope, MScopeListDto>();

            CreateMap<Storage.Entities.Scope, MScopeDto>();

            CreateMap<MScopeDto, Storage.Entities.Scope>();
        }
    }
}
