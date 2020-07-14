using AutoMapper;
using middler.Common.SharedModels.Models;
using middlerApp.API.Helper;
using middlerApp.API.MapperProfiles.Formatters;
using middlerApp.SharedModels;

namespace middlerApp.API.MapperProfiles
{
    public class EndpointActionProfile: Profile
    {
        public EndpointActionProfile()
        {
            CreateMap(typeof(API.DataAccess.EndpointActionEntity), typeof(MiddlerAction<>))
                .ForMember("Parameters", mbr => mbr.ConvertUsing(new StringToJObjectFormatter(), "Parameters"));

            CreateMap(typeof(MiddlerAction<>), typeof(API.DataAccess.EndpointActionEntity))
                .ForMember("Parameters", mbr => mbr.ConvertUsing(new StringToJObjectFormatter(), "Parameters"));

            CreateMap<API.DataAccess.EndpointActionEntity, MiddlerAction>();

            CreateMap<API.DataAccess.EndpointActionEntity, EndpointActionDto>()
                .ForMember(dest => dest.Parameters, expression => expression.MapFrom(dto => Converter.Json.ToDictionary(dto.Parameters)));

            CreateMap<EndpointActionDto, API.DataAccess.EndpointActionEntity>()
                .ForMember(dest => dest.Parameters, expression => expression.MapFrom(dto => Converter.Json.ToJObject(dto.Parameters)));

            CreateMap<API.DataAccess.EndpointActionEntity, EndpointRuleListActionDto>();

            CreateMap<API.DataAccess.EndpointActionEntity, MiddlerAction>();
        }
    }
}
