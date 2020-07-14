using AutoMapper;
using IdentityServer4.Models;

namespace middlerApp.API.IDP.Storage.Mappers
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
