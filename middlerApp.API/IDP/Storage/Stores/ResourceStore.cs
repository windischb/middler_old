﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using middlerApp.API.IDP.Mappers;
using middlerApp.API.IDP.Storage.Entities;
using ApiResource = IdentityServer4.Models.ApiResource;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace middlerApp.API.IDP.Storage.Stores
{
    /// <summary>
    /// Implementation of IResourceStore thats uses EF.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        /// <summary>
        /// The DbContext.
        /// </summary>
        protected readonly IDPDbContext Context;
        
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<ResourceStore> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public ResourceStore(IDPDbContext context, ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames == null) throw new ArgumentNullException(nameof(apiResourceNames));

            var query =
                from apiResource in Context.ApiResources
                where apiResourceNames.Contains(apiResource.Name)
                select apiResource;

            var apis = query
                .Include(x => x.Secrets)
                .Include(x => x.Scopes).ThenInclude(s => s.Scope).ThenInclude(s => s.UserClaims)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var result = (await apis.ToArrayAsync()).Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                Logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                Logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }

            return result;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var query =
                from api in Context.ApiResources
                where api.Scopes.Where(x => names.Contains(x.Scope.Name)).Any()
                select api;

            var apis = Context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.Scopes).ThenInclude(s => s.Scope).ThenInclude(s => s.UserClaims)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = await apis.ToArrayAsync();
            var models = results.Select(x => x.ToModel()).ToArray();

            Logger.LogDebug("Found {apis} API resources in database", models.Select(x => x.Name));

            return models;
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from identityResource in Context.Scopes.WhereIsIdentityResource()
                where scopes.Contains(identityResource.Name)
                select identityResource;

            var resources = query
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = await resources.ToArrayAsync();

            Logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            var arr = results.Select(x => x.ToIdentityResourceModel()).ToArray();
            return arr;
        }

        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from scope in Context.Scopes.WhereIsApiScope()
                where scopes.Contains(scope.Name)
                select scope;

            var resources = query
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = await resources.ToArrayAsync();

            Logger.LogDebug("Found {scopes} scopes in database", results.Select(x => x.Name));

            var arr = results.Select(x => x.ToApiScopeModel()).ToArray();
            return arr;
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = Context.Scopes.WhereIsIdentityResource()
              .Include(x => x.UserClaims)
              .Include(x => x.Properties);

            var apis = Context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.Scopes).ThenInclude(s => s.Scope)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();
            
            var scopes = Context.Scopes.WhereIsApiScope()
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var result = new Resources(
                (await identity.ToArrayAsync()).Select(x => x.ToIdentityResourceModel()),
                (await apis.ToArrayAsync()).Select(x => x.ToModel()),
                (await scopes.ToArrayAsync()).Select(x => x.ToApiScopeModel())
            );

            Logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources", 
                result.IdentityResources.Select(x=>x.Name).Union(result.ApiScopes.Select(x=>x.Name)),
                result.ApiResources.Select(x=>x.Name));

            return result;
        }
    }
}