// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using middler.IDP.Storage.Entities;

namespace middler.IDP.Storage.Mappers
{

    public static class AuthorizationCodeMapper
    {
        static AuthorizationCodeMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<AuthorizationCodeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }


        public static IdentityServer4.Models.AuthorizationCode ToModel(this Entities.AuthorizationCode entity)
        {
            return entity == null ? null : Mapper.Map<IdentityServer4.Models.AuthorizationCode>(entity);
        }

        public static AuthorizationCode ToEntity(this IdentityServer4.Models.AuthorizationCode model)
        {
            return model == null ? null : Mapper.Map<AuthorizationCode>(model);
        }
    }
}