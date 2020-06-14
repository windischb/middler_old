using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using middler.Common.SharedModels.Models;
using middlerApp.API.MapperProfiles;
using middlerApp.SharedModels;

namespace middlerApp.Data.MapperProfiles
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

            CreateMap<EndpointRuleEntity, EndpointRuleListDto>()
                .ForMember(dto => dto.Actions,
                    expression => expression.MapFrom(entity => entity.Actions.OrderBy(a => a.Order)))
                .ForMember(
                    dest => dest.Scheme,
                    expression => expression.MapFrom(src => MappingHelper.Split(src.Scheme)))
                .ForMember(
                    dest => dest.HttpMethods,
                    expression => expression.MapFrom(src => MappingHelper.Split(src.HttpMethods)));

            CreateMap<EndpointRuleEntity, EndpointRuleDto>()
                .ForMember(
                    dto => dto.Scheme,
                    opts => opts.MapFrom((dbModel) => MappingHelper.Split(dbModel.Scheme)))
                .ForMember(
                    dto => dto.HttpMethods,
                    opts => opts.MapFrom((dbModel) => MappingHelper.Split(dbModel.HttpMethods)));

            CreateMap<EndpointRuleDto, EndpointRuleEntity>()
                .ForMember(
                    dto => dto.Scheme,
                    opts => opts.MapFrom((dbModel) => String.Join("; ", dbModel.Scheme)))
                .ForMember(
                    dto => dto.HttpMethods,
                    opts => opts.MapFrom((dbModel) => String.Join("; ", dbModel.HttpMethods)));

        }

    }
}
