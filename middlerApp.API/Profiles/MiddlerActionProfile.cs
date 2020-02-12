using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using middler.Common.SharedModels.Models;
using middler.Hosting.Models;
using Reflectensions.ExtensionMethods;

namespace middlerApp.API.Profiles {
    public class MiddlerActionProfile: Profile {

        public MiddlerActionProfile() {

            //CreateMap<MiddlerAction, MiddlerActionDto>()
            //    .ForMember(dto => dto.ActionType, opts => opts.MapFrom((dbModel) => dbModel.ActionType.Split('.', StringSplitOptions.None).Last()));

            CreateMap(typeof(MiddlerAction<>), typeof(MiddlerActionDto))
                .ForMember("ActionType", expression => expression.MapFrom(act => act.GetPropertyValue<string>("ActionType", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public).Split('.', StringSplitOptions.None).Last()));

            

        }
    }
}
