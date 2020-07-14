using AutoMapper;

namespace middlerApp.API.IDP.Storage.Mappers
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
