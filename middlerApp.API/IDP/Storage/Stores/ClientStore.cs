﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using middlerApp.API.IDP.Mappers;

namespace middlerApp.API.IDP.Storage.Stores
{
    /// <summary>
    /// Implementation of IClientStore thats uses EF.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IClientStore" />
    public class ClientStore : IClientStore
    {

        protected readonly IDPDbContext Context;


        protected readonly ILogger<ClientStore> Logger;


        public ClientStore(IDPDbContext context, ILogger<ClientStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger;
        }


        public virtual async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
        {
            IQueryable<Entities.Client> baseQuery = Context.Clients
                .Where(x => x.ClientId == clientId)
                .Take(1);

            var client = await baseQuery.FirstOrDefaultAsync();
            if (client == null) return null;

            await baseQuery.Include(x => x.AllowedCorsOrigins).SelectMany(c => c.AllowedCorsOrigins).LoadAsync();
            await baseQuery.Include(x => x.AllowedGrantTypes).SelectMany(c => c.AllowedGrantTypes).LoadAsync();
            await baseQuery.Include(x => x.AllowedScopes).ThenInclude(s => s.Scope).SelectMany(c => c.AllowedScopes).LoadAsync();
            await baseQuery.Include(x => x.Claims).SelectMany(c => c.Claims).LoadAsync();
            await baseQuery.Include(x => x.ClientSecrets).SelectMany(c => c.ClientSecrets).LoadAsync();
            await baseQuery.Include(x => x.IdentityProviderRestrictions).SelectMany(c => c.IdentityProviderRestrictions).LoadAsync();
            await baseQuery.Include(x => x.PostLogoutRedirectUris).SelectMany(c => c.PostLogoutRedirectUris).LoadAsync();
            await baseQuery.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();
            await baseQuery.Include(x => x.RedirectUris).SelectMany(c => c.RedirectUris).LoadAsync();

            var model = client.ToModel();

            Logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

            return model;
        }
    }
}