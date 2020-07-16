// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

#pragma warning disable 1591

namespace middlerApp.API.IDP.Storage.Entities
{
    public class ApiResourceScope
    {
        public Guid Id { get; set; }
        public string Scope { get; set; }

        public Guid ApiResourceId { get; set; }
        public ApiResource ApiResource { get; set; }
    }
}