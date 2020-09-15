using System;
using System.Linq;
using AutoMapper;
using IdentityServer4.Models;

namespace middlerApp.API.IDP.Mappers
{
    public class UserConsentMapperProfile: Profile
    {
        public UserConsentMapperProfile()
        {
            CreateMap<Storage.Entities.UserConsent, Consent>()
                .ForMember(dest => dest.Scopes,
                    expression => expression.MapFrom((src, dest) =>
                        src.Scopes?.Split(";").Select(s => s.Trim()).Where(s => !String.IsNullOrWhiteSpace(s))));

            CreateMap<Consent, Storage.Entities.UserConsent>()
                .ForMember(dest => dest.Scopes,
                    expression => expression.MapFrom((src, dest) => String.Join(';', src.Scopes)));



        }
    }
}
