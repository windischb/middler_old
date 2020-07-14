using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using middler.Common.SharedModels.Models;


namespace middlerApp.Data.MapperProfiles
{
    internal class EndpointActionProfile: Profile
    {
        public EndpointActionProfile()
        {
            CreateMap<EndpointActionEntity, MiddlerAction>();
        }
    }
}
