using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using middler.Common;
using middler.Common.SharedModels.Models;
using middler.Common.Storage;
using middler.Hosting.Models;
using middler.Storage.LiteDB;
using Reflectensions.ExtensionMethods;

namespace middler.Hosting.Profiles {
    public class MiddlerActionProfile: Profile {

        public MiddlerActionProfile() {

            //CreateMap<MiddlerAction, MiddlerActionDto>()
            //    .ForMember(dto => dto.ActionType, opts => opts.MapFrom((dbModel) => dbModel.ActionType.Split('.', StringSplitOptions.None).Last()));

            CreateMap(typeof(MiddlerAction<>), typeof(MiddlerActionDto))
                .ForMember("ActionType", expression => expression.MapFrom(act => act.GetPropertyValue<string>("ActionType", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public).Split('.', StringSplitOptions.None).Last()));

            

        }
    }
}
