using AutoMapper;
using middler.Common.SharedModels.Models;

namespace middlerApp.Data.MapperProfiles
{
    internal class EndpointRuleProfile : Profile
    {
        public EndpointRuleProfile()
        {
            CreateMap<EndpointRuleEntity, MiddlerRule>()
                .ForMember(
                    dest => dest.Scheme,
                    expression => expression.MapFrom(src => MappingHelper.Split(src.Scheme)))
                .ForMember(
                    dest => dest.HttpMethods,
                    expression => expression.MapFrom(src => MappingHelper.Split(src.HttpMethods)));

            //CreateMap<MiddlerRule, EndpointRuleEntity>()
            //    .ForMember(
            //        dest => dest.Scheme,
            //        expression => expression.MapFrom(src => String.Join("; ", src.Scheme)))
            //    .ForMember(
            //        dest => dest.HttpMethods,
            //        expression => expression.MapFrom(src => String.Join("; ", src.HttpMethods)));


        }

    }
}
