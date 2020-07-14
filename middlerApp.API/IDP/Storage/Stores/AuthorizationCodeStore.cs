//using System.Threading.Tasks;
//using IdentityServer4.Models;
//using IdentityServer4.Services;
//using IdentityServer4.Stores;
//using Microsoft.EntityFrameworkCore;
//using middlerApp.API.IDP.Storage.Mappers;

//namespace middlerApp.API.IDP.Storage.Stores
//{
//    public class AuthorizationCodeStore : IAuthorizationCodeStore
//    {
//        public IHandleGenerationService HandleGenerationService { get; }

//        public IDPDbContext DbContext { get; }


//        protected AuthorizationCodeStore(IDPDbContext dbContext)
//        {
//            HandleGenerationService = new DefaultHandleGenerationService();
//            DbContext = dbContext;
//        }

//        public async Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
//        {
//            var handle = await HandleGenerationService.GenerateAsync();
//            var entity = code.ToEntity();
//            entity.Id = handle.Sha256();

//            await DbContext.AuthorizationCodes.AddAsync(entity);
//            await DbContext.SaveChangesAsync();
//            return handle;
//        }

//        public async Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
//        {
//            var hashedKey = code.Sha256();
//            var entity = await DbContext.AuthorizationCodes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == hashedKey);
//            return entity?.ToModel();
//        }

//        public async Task RemoveAuthorizationCodeAsync(string code)
//        {
//            var hashedKey = code.Sha256();
//            var entity = await DbContext.AuthorizationCodes.FirstOrDefaultAsync(a => a.Id == hashedKey);
//            DbContext.AuthorizationCodes.Remove(entity);
//            await DbContext.SaveChangesAsync();
//        }

//    }
//}
