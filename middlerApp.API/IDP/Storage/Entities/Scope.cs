// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace middlerApp.API.IDP.Storage.Entities
{
    public class Scope
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public string Type { get; set; }
        public List<ScopeClaim> UserClaims { get; set; }
        public List<ScopeProperty> Properties { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public bool NonEditable { get; set; }

        public List<ApiResourceScope> ApiResources { get; set; } = new List<ApiResourceScope>();
        public List<ClientScope> Clients { get; set; } = new List<ClientScope>();
    }

    public static class ScopeType
    {
        public static string ApiScope => "ApiScope";

        public static string IdentityResource => "IdentityResource"; 
    }

    public static class ScopeDbSetExtensions
    {
        public static IQueryable<Scope> WithType(this IQueryable<Scope> scopes, string scopeType)
        {
            return scopes.Where(s => s.Type == scopeType);
        }

        public static IQueryable<Scope> WhereIsApiScope(this IQueryable<Scope> scopes)
        {
            return scopes.WithType(ScopeType.ApiScope);
        }

        public static IQueryable<Scope> WhereIsIdentityResource(this IQueryable<Scope> scopes)
        {
            return scopes.WithType(ScopeType.IdentityResource);
        }
    }
}