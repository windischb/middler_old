using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;

namespace middler.IDP.Storage.Mappers
{
    public class UserConsentMapperProfile: Profile
    {
        public UserConsentMapperProfile()
        {
            CreateMap<Entities.UserConsent, Consent>()
                .ReverseMap();
        }
    }
}
