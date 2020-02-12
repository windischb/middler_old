using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using middler.Common.Storage;
using middler.Hosting.Models;
using middler.Storage.LiteDB;

namespace middler.Hosting.Profiles {
    public class MiddlerRuleProfile: Profile {

        public MiddlerRuleProfile() {

            CreateMap<CreateMiddlerRuleDto, MiddlerRuleDbModel>()
                .ForMember(dto => dto.Scheme, opts => opts.MapFrom((dbModel) => dbModel.Scheme.Distinct()))
                .ForMember(dto => dto.HttpMethods, opts => opts.MapFrom((dbModel) => dbModel.HttpMethods.Distinct()));

            CreateMap<UpdateMiddlerRuleDto, MiddlerRuleDbModel>()
                .ForMember(dto => dto.Scheme, opts => opts.MapFrom((dbModel) => dbModel.Scheme.Distinct()))
                .ForMember(dto => dto.HttpMethods, opts => opts.MapFrom((dbModel) => dbModel.HttpMethods.Distinct()));

            CreateMap<MiddlerRuleDbModel, UpdateMiddlerRuleDto>()
                .ForMember(dto => dto.Scheme, opts => opts.MapFrom((dbModel) => dbModel.Scheme.Distinct()))
                .ForMember(dto => dto.HttpMethods, opts => opts.MapFrom((dbModel) => dbModel.HttpMethods.Distinct()));

            CreateMap<MiddlerRuleDbModel, MiddlerRuleDto>()
                .ForMember(dto => dto.Scheme, opts => opts.MapFrom((dbModel) => dbModel.Scheme.Distinct()))
                .ForMember(dto => dto.HttpMethods, opts => opts.MapFrom((dbModel) => dbModel.HttpMethods.Distinct()));
        }
    }
}
