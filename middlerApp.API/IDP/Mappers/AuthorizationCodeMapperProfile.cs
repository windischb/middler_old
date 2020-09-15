using AutoMapper;

namespace middlerApp.API.IDP.Mappers
{
    public class AuthorizationCodeMapperProfile: Profile
    {
        public AuthorizationCodeMapperProfile()
        {
            CreateMap<Storage.Entities.AuthorizationCode, IdentityServer4.Models.AuthorizationCode>()
                .ReverseMap();
        }
    }
}
