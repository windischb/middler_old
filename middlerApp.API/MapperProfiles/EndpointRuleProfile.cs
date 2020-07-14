using System;
using System.Linq;
using AutoMapper;
using middler.Common.SharedModels.Models;
using middlerApp.SharedModels;

namespace middlerApp.API.MapperProfiles
{
    public class EndpointRuleProfile : Profile
    {
        public EndpointRuleProfile()
        {
            //CreateMap<EndpointRuleEntity, MiddlerRule>()
            //    .ForMember(
            //        dest => dest.Scheme, 
            //        expression => expression.MapFrom(src => Split(src.Scheme)))
            //    .ForMember(
            //        dest => dest.HttpMethods,
            //        expression => expression.MapFrom(src => Split(src.HttpMethods)));

            //CreateMap<MiddlerRule, EndpointRuleEntity>()
            //    .ForMember(
            //        dest => dest.Scheme,
            //        expression => expression.MapFrom(src => String.Join("; ", src.Scheme)))
            //    .ForMember(
            //        dest => dest.HttpMethods,
            //        expression => expression.MapFrom(src => String.Join("; ", src.HttpMethods)));

            CreateMap<API.DataAccess.EndpointRuleEntity, EndpointRuleListDto>()
                .ForMember(dto => dto.Actions,
                    expression => expression.MapFrom(entity => entity.Actions.OrderBy(a => a.Order)))
                .ForMember(
                    dest => dest.Scheme,
                    expression => expression.MapFrom(src => DataAccess.MappingHelper.Split(src.Scheme)))
                .ForMember(
                    dest => dest.HttpMethods,
                    expression => expression.MapFrom(src => DataAccess.MappingHelper.Split(src.HttpMethods)));

            CreateMap<API.DataAccess.EndpointRuleEntity, EndpointRuleDto>()
                .ForMember(
                    dto => dto.Scheme,
                    opts => opts.MapFrom((dbModel) => DataAccess.MappingHelper.Split(dbModel.Scheme)))
                .ForMember(
                    dto => dto.HttpMethods,
                    opts => opts.MapFrom((dbModel) => DataAccess.MappingHelper.Split(dbModel.HttpMethods)));

            CreateMap<EndpointRuleDto, API.DataAccess.EndpointRuleEntity>()
                .ForMember(
                    dto => dto.Scheme,
                    opts => opts.MapFrom((dbModel) => String.Join("; ", dbModel.Scheme)))
                .ForMember(
                    dto => dto.HttpMethods,
                    opts => opts.MapFrom((dbModel) => String.Join("; ", dbModel.HttpMethods)));


            CreateMap<API.DataAccess.EndpointRuleEntity, MiddlerRule>()
                .ForMember(
                    dest => dest.Scheme,
                    expression => expression.MapFrom(src => DataAccess.MappingHelper.Split(src.Scheme)))
                .ForMember(
                    dest => dest.HttpMethods,
                    expression => expression.MapFrom(src => DataAccess.MappingHelper.Split(src.HttpMethods)));

        }

    }
}
