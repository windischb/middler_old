// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using middler.IDP.Storage.Entities;

namespace middler.IDP.Storage.Mappers
{

    public static class UserConsentMapper
    {
        static UserConsentMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserConsentMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }


        public static IdentityServer4.Models.Consent ToModel(this UserConsent entity)
        {
            return entity == null ? null : Mapper.Map<IdentityServer4.Models.Consent>(entity);
        }

        public static UserConsent ToEntity(this IdentityServer4.Models.Consent model)
        {
            return model == null ? null : Mapper.Map<UserConsent>(model);
        }
    }
}