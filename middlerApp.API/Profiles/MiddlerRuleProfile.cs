using System.Linq;
using AutoMapper;
using middler.Common.Storage;
using middler.Hosting.Models;

namespace middlerApp.API.Profiles {
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
