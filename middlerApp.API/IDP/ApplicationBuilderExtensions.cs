using Microsoft.AspNetCore.Builder;

namespace middlerApp.API.IDP
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMiddlerIdentityServer(this IApplicationBuilder app)
        {
            //InitializeDatabase(app);
            app.UseIdentityServer();
        }

        //private static void InitializeDatabase(IApplicationBuilder app)
        //{
        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        //        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        //        context.Database.Migrate();
                
        //    }
        //}
    }
}
