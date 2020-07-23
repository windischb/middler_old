// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Storage.Entities;
using middlerApp.API.IDP.Storage.Mappers;

namespace middlerApp.API.IDP.Mappers
{

    public class SecretMapperProfile : Profile
    {

        public SecretMapperProfile()
        {
            CreateMap<Storage.Entities.Secret, SecretDto>();

            CreateMap<SecretDto, ApiResourceSecret>();

            CreateMap<SecretDto, ClientSecret>();
        }
    }
}
