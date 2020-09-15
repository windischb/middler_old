﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using middlerApp.API.IDP.Storage.Entities;

namespace middlerApp.API.IDP.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for persisted grants.
    /// </summary>
    public static class PersistedGrantMappers
    {
        static PersistedGrantMappers()
        {
            Mapper = new MapperConfiguration(cfg =>cfg.AddProfile<PersistedGrantMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static IdentityServer4.Models.PersistedGrant ToModel(this PersistedGrant entity)
        {
            return entity == null ? null : Mapper.Map<IdentityServer4.Models.PersistedGrant>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Storage.Entities.PersistedGrant ToEntity(this IdentityServer4.Models.PersistedGrant model)
        {
            return model == null ? null : Mapper.Map<Storage.Entities.PersistedGrant>(model);
        }

        /// <summary>
        /// Updates an entity from a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="entity">The entity.</param>
        public static void UpdateEntity(this IdentityServer4.Models.PersistedGrant model, Storage.Entities.PersistedGrant entity)
        {
            Mapper.Map(model, entity);
        }
    }
}