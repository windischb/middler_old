// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

#pragma warning disable 1591

namespace middlerApp.API.IDP.Storage.Entities
{
    public class ApiScopeProperty : Property
    {
        public Guid ScopeId { get; set; }
        public ApiScope Scope { get; set; }
    }
}