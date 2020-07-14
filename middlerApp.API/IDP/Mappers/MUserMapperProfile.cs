using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middlerApp.API.IDP.DtoModels;
using middlerApp.API.IDP.Models;

namespace middlerApp.API.IDP.Mappers
{
    public class MUserMapperProfile : Profile
    {
        public MUserMapperProfile()
        {
            CreateMap<MUserDto, MUser>()
                .ForMember(dest => dest.UserRoles, expression => expression.MapFrom((dto, user) => dto.Roles.Select(r => new MUserRoles(){RoleId = r.Id})));

            CreateMap<MUser, MUserDto>()
                .ForMember(dest => dest.Roles, expression => expression.MapFrom((user, dto) => user.UserRoles.Select(ur => ur.Role)));

            CreateMap<MUserClaim, MUserClaimDto>().ReverseMap();

            CreateMap<MUser, MUserListDto>();

        }
    }
}
