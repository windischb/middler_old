using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Stores.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using middlerApp.API.IDP.Models;
using middlerApp.API.IDP.Storage.Entities;
using Newtonsoft.Json;

namespace middlerApp.API.IDP
{
    public class IDPDbContext : DbContext
    {
        public DbSet<MUser> Users { get; set; }
        public DbSet<MRole> Roles { get; set; }

        public DbSet<MUserClaim> UserClaims { get; set; }

        public DbSet<MUserLogin> UserLogins { get; set; }

        public DbSet<MUserSecret> UserSecrets { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }
        //public DbSet<IdentityResource> IdentityResources { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<Scope> Scopes { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public DbSet<UserConsent> UserConsents { get; set; }
        //public DbSet<AuthorizationCode> AuthorizationCodes { get; set; }


        public IDPDbContext(DbContextOptions<IDPDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            ConfigureUserAndRoles(modelBuilder);
          
            ConfigureClientContext(modelBuilder);
            ConfigureResourcesContext(modelBuilder);
            ConfigurePersistedGrantContext(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified).Select(e => e.Entity)
                    .OfType<IConcurrencyAware>();

            var entries = ChangeTracker.Entries();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static void ConfigureClientContext(ModelBuilder modelBuilder)
        {
            //if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) 
            //    modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable("Clients");
                client.HasKey(x => x.Id);

                client.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                client.Property(x => x.ProtocolType).HasMaxLength(200).IsRequired();
                client.Property(x => x.ClientName).HasMaxLength(200);
                client.Property(x => x.ClientUri).HasMaxLength(2000);
                client.Property(x => x.LogoUri).HasMaxLength(2000);
                client.Property(x => x.Description).HasMaxLength(1000);
                client.Property(x => x.FrontChannelLogoutUri).HasMaxLength(2000);
                client.Property(x => x.BackChannelLogoutUri).HasMaxLength(2000);
                client.Property(x => x.ClientClaimsPrefix).HasMaxLength(200);
                client.Property(x => x.PairWiseSubjectSalt).HasMaxLength(200);
                client.Property(x => x.UserCodeType).HasMaxLength(100);
                client.Property(x => x.AllowedIdentityTokenSigningAlgorithms).HasMaxLength(100);

                client.HasIndex(x => x.ClientId).IsUnique();

                client.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.RedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Claims).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Properties).WithOne(x => x.Client).HasForeignKey(x => x.ClientId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                
            });

            modelBuilder.Entity<ClientGrantType>(grantType =>
            {
                grantType.ToTable("ClientGrantTypes");
                grantType.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientRedirectUri>(redirectUri =>
            {
                redirectUri.ToTable("ClientRedirectUris");
                redirectUri.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(postLogoutRedirectUri =>
            {
                postLogoutRedirectUri.ToTable("ClientPostLogoutRedirectUris");
                postLogoutRedirectUri.Property(x => x.PostLogoutRedirectUri).HasMaxLength(2000).IsRequired();
            });

            //modelBuilder.Entity<ClientScope>(scope =>
            //{
            //    scope.ToTable("ClientScopes");
            //    scope.Property(x => x.Scope).HasMaxLength(200).IsRequired();
            //});

            modelBuilder.Entity<ClientScope>().HasKey(e => new { e.ScopeId, e.ClientId });

            modelBuilder.Entity<ClientScope>()
                .HasOne(t => t.Client)
                .WithMany(t => t.AllowedScopes)
                .HasForeignKey(t => t.ClientId);

            modelBuilder.Entity<ClientScope>()
                .HasOne(t => t.Scope)
                .WithMany(t => t.Clients)
                .HasForeignKey(t => t.ScopeId);


            modelBuilder.Entity<ClientSecret>(secret =>
            {
                secret.ToTable("ClientSecrets");
                secret.Property(x => x.Value).HasMaxLength(4000).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(250).IsRequired();
                secret.Property(x => x.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<ClientClaim>(claim =>
            {
                claim.ToTable("ClientClaims");
                claim.Property(x => x.Type).HasMaxLength(250).IsRequired();
                claim.Property(x => x.Value).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientIdPRestriction>(idPRestriction =>
            {
                idPRestriction.ToTable("ClientIdPRestrictions");
                idPRestriction.Property(x => x.Provider).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientCorsOrigin>(corsOrigin =>
            {
                corsOrigin.ToTable("ClientCorsOrigins");
                corsOrigin.Property(x => x.Origin).HasMaxLength(150).IsRequired();
            });

            modelBuilder.Entity<ClientProperty>(property =>
            {
                property.ToTable("ClientProperties");
                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });
        }

        private static void ConfigureResourcesContext(ModelBuilder modelBuilder)
        {
            //if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema))
            //    modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<Scope>(identityResource =>
            {
                //identityResource.ToTable("IdentityResources").HasKey(x => x.Id);

                identityResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                identityResource.Property(x => x.DisplayName).HasMaxLength(200);
                identityResource.Property(x => x.Description).HasMaxLength(1000);

                identityResource.HasIndex(x => x.Name).IsUnique();

                identityResource.HasMany(x => x.UserClaims).WithOne(x => x.Scope).HasForeignKey(x => x.ScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                identityResource.HasMany(x => x.Properties).WithOne(x => x.Scope).HasForeignKey(x => x.ScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScopeClaim>(claim =>
            {
                //claim.ToTable("IdentityResourceClaims").HasKey(x => x.Id);

                claim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ScopeProperty>(property =>
            {
                //property.ToTable("IdentityResourceProperties");
                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });



            modelBuilder.Entity<ApiResource>(apiResource =>
            {
                apiResource.ToTable("ApiResources").HasKey(x => x.Id);

                apiResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                apiResource.Property(x => x.DisplayName).HasMaxLength(200);
                apiResource.Property(x => x.Description).HasMaxLength(1000);
                apiResource.Property(x => x.AllowedAccessTokenSigningAlgorithms).HasMaxLength(100);

                apiResource.HasIndex(x => x.Name).IsUnique();

                apiResource.HasMany(x => x.Secrets).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.Scopes).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.UserClaims).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.Properties).WithOne(x => x.ApiResource).HasForeignKey(x => x.ApiResourceId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiResourceSecret>(apiSecret =>
            {
                apiSecret.ToTable("ApiResourceSecrets").HasKey(x => x.Id);

                apiSecret.Property(x => x.Description).HasMaxLength(1000);
                apiSecret.Property(x => x.Value).HasMaxLength(4000).IsRequired();
                apiSecret.Property(x => x.Type).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ApiResourceClaim>(apiClaim =>
            {
                apiClaim.ToTable("ApiResourceClaims").HasKey(x => x.Id);

                apiClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            //modelBuilder.Entity<ApiResourceScope>((System.Action<EntityTypeBuilder<ApiResourceScope>>)(apiScope =>
            //{
            //    apiScope.ToTable("ApiResourceScopes").HasKey(x => x.Id);

            //    apiScope.Property(x => x.Scope).HasMaxLength(200).IsRequired();
            //}));

            modelBuilder.Entity<ApiResourceScope>().HasKey(e => new { e.ScopeId, e.ApiResourceId });

            modelBuilder.Entity<ApiResourceScope>()
                .HasOne(t => t.ApiResource)
                .WithMany(t => t.Scopes)
                .HasForeignKey(t => t.ApiResourceId);

            modelBuilder.Entity<ApiResourceScope>()
                .HasOne(t => t.Scope)
                .WithMany(t => t.ApiResources)
                .HasForeignKey(t => t.ScopeId);

            modelBuilder.Entity<ApiResourceProperty>(property =>
            {
                property.ToTable("ApiResourceProperties");
                property.Property(x => x.Key).HasMaxLength(250).IsRequired();
                property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            });


            modelBuilder.Entity<Scope>(scope =>
            {
                //scope.ToTable("ApiScopes").HasKey(x => x.Id);

                scope.Property(x => x.Name).HasMaxLength(200).IsRequired();
                scope.Property(x => x.DisplayName).HasMaxLength(200);
                scope.Property(x => x.Description).HasMaxLength(1000);

                scope.HasIndex(x => x.Name).IsUnique();

                scope.HasMany(x => x.UserClaims).WithOne(x => x.Scope).HasForeignKey(x => x.ScopeId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });
            //modelBuilder.Entity<ApiScopeClaim>(scopeClaim =>
            //{
            //    //scopeClaim.ToTable("ApiScopeClaims").HasKey(x => x.Id);

            //    scopeClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            //});
            //modelBuilder.Entity<ApiScopeProperty>(property =>
            //{
            //    property.ToTable("ApiScopeProperties").HasKey(x => x.Id);
            //    property.Property(x => x.Key).HasMaxLength(250).IsRequired();
            //    property.Property(x => x.Value).HasMaxLength(2000).IsRequired();
            //});


        }

        private static void ConfigurePersistedGrantContext(ModelBuilder modelBuilder)
        {
            //if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema))
            //    modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<PersistedGrant>(grant =>
            {
                grant.ToTable("PersistedGrants");

                grant.Property(x => x.Key).HasMaxLength(200).ValueGeneratedNever();
                grant.Property(x => x.Type).HasMaxLength(50).IsRequired();
                grant.Property(x => x.SubjectId).HasMaxLength(200);
                grant.Property(x => x.SessionId).HasMaxLength(100);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.Description).HasMaxLength(200);
                grant.Property(x => x.CreationTime).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                grant.Property(x => x.Data).HasMaxLength(50000).IsRequired();

                grant.HasKey(x => x.Key);

                grant.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
                grant.HasIndex(x => new { x.SubjectId, x.SessionId, x.Type });
                grant.HasIndex(x => x.Expiration);
            });

            modelBuilder.Entity<DeviceFlowCodes>(codes =>
            {
                codes.ToTable("DeviceFlowCodes");

                codes.Property(x => x.DeviceCode).HasMaxLength(200).IsRequired();
                codes.Property(x => x.UserCode).HasMaxLength(200).IsRequired();
                codes.Property(x => x.SubjectId).HasMaxLength(200);
                codes.Property(x => x.SessionId).HasMaxLength(100);
                codes.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                codes.Property(x => x.Description).HasMaxLength(200);
                codes.Property(x => x.CreationTime).IsRequired();
                codes.Property(x => x.Expiration).IsRequired();
                // 50000 chosen to be explicit to allow enough size to avoid truncation, yet stay beneath the MySql row size limit of ~65K
                // apparently anything over 4K converts to nvarchar(max) on SqlServer
                codes.Property(x => x.Data).HasMaxLength(50000).IsRequired();

                codes.HasKey(x => new { x.UserCode });

                codes.HasIndex(x => x.DeviceCode).IsUnique();
                codes.HasIndex(x => x.Expiration);
            });

            var valueComparer = new ValueComparer<IEnumerable<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());



            //modelBuilder.Entity<UserConsent>().Property(p => p.Scopes)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<List<string>>(v))
            //    .Metadata.SetValueComparer(valueComparer);



            //modelBuilder.Entity<AuthorizationCode>().Property(p => p.RequestedScopes)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<List<string>>(v))
            //    .Metadata.SetValueComparer(valueComparer);

            //modelBuilder.Entity<AuthorizationCode>().Property(p => p.Properties)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));

            //modelBuilder.Entity<AuthorizationCode>().Property(p => p.Subject)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(v, new ClaimConverter()),
            //        v => JsonConvert.DeserializeObject<ClaimsPrincipal>(v, new ClaimConverter()));
        }

        private static void ConfigureUserAndRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MUser>()
                .HasIndex(u => u.Subject)
                .IsUnique();

            modelBuilder.Entity<MUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<MUserRoles>().HasKey(e => new {e.UserId, e.RoleId});

            modelBuilder.Entity<MUserRoles>()
                .HasOne(t => t.User)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<MUserRoles>()
                .HasOne(t => t.Role)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(t => t.RoleId);
        }
    }
}
