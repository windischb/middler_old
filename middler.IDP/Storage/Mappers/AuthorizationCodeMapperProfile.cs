using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace middler.IDP.Storage.Mappers
{
    public class AuthorizationCodeMapperProfile: Profile
    {
        public AuthorizationCodeMapperProfile()
        {
            CreateMap<Entities.AuthorizationCode, IdentityServer4.Models.AuthorizationCode>()
                .ReverseMap();
        }
    }
}
