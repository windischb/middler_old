// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Mappers
{
    /// <summary>
    /// Defines entity/model mapping for API resources.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class ApiResourceMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="ApiResourceMapperProfile"/>
        /// </summary>
        public ApiResourceMapperProfile()
        {
            CreateMap<Storage.Entities.ApiResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Storage.Entities.ApiResource, IdentityServer4.Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new IdentityServer4.Models.ApiResource())
                .ForMember(x => x.ApiSecrets, opts => opts.MapFrom(x => x.Secrets))
                .ForMember(dest => dest.Scopes, expression => expression.MapFrom((entity, client) =>
                {
                    return entity.Scopes.Select(r => r.Scope.Name);
                }))
                .ForMember(x=>x.AllowedAccessTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x=>x.AllowedAccessTokenSigningAlgorithms))
                .ReverseMap()
                .ForMember(x => x.AllowedAccessTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedAccessTokenSigningAlgorithms));

            CreateMap<Storage.Entities.ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<Storage.Entities.ApiResourceSecret, IdentityServer4.Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<Storage.Entities.ApiResourceScope, Scope>()
                .ConstructUsing(x => x.Scope)
                .ReverseMap()
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

            CreateMap<Storage.Entities.ApiResource, MApiResourceListDto>();

            CreateMap<Storage.Entities.ApiResource, MApiResourceDto>()
                .ForMember(dest => dest.Scopes, expression => expression.MapFrom((client, dto) => client.Scopes.Select(r => r.Scope)));

            CreateMap<MApiResourceDto, Storage.Entities.ApiResource>()
                .ForMember(dest => dest.Scopes, expression => expression.MapFrom((dto, resource) => dto.Scopes.Select(r => new ApiResourceScope() { ScopeId = r.Id })))
                ;

        }
    }
}
