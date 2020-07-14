using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using middlerApp.API.IDP.Storage.Mappers;

namespace middlerApp.API.IDP.Storage.Stores
{
    public class UserConsentStore : IUserConsentStore
    {
        public IDPDbContext DbContext { get; }

        public UserConsentStore(IDPDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task StoreUserConsentAsync(Consent consent)
        {
            var entity = consent.ToEntity();
            await DbContext.UserConsents.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
        {
            var userConsent = await DbContext.UserConsents.AsNoTracking().FirstOrDefaultAsync(c => c.SubjectId == subjectId && c.ClientId == clientId);
            return userConsent.ToModel();
        }

        public async Task RemoveUserConsentAsync(string subjectId, string clientId)
        {
            var userConsent = await DbContext.UserConsents.FirstOrDefaultAsync(c => c.SubjectId == subjectId && c.ClientId == clientId);
            DbContext.Remove(userConsent);
            await DbContext.SaveChangesAsync();
        }
    }
}
