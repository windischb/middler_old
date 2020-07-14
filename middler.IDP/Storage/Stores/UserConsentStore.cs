using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using middler.IDP.DbContexts;
using middler.IDP.Storage.Mappers;

namespace middler.IDP.Storage.Stores
{
    //public class UserConsentStore: IUserConsentStore
    //{
    //    public IdentityDbContext DbContext { get; }

    //    public UserConsentStore(IdentityDbContext dbContext)
    //    {
    //        DbContext = dbContext;
    //    }
    //    public async Task StoreUserConsentAsync(Consent consent)
    //    {
    //        var entity = consent.ToEntity();
    //        await DbContext.UserConsents.AddAsync(entity);
    //        await DbContext.SaveChangesAsync();
    //    }

    //    public async Task<Consent> GetUserConsentAsync(string subjectId, string clientId)
    //    {
    //        var userConsent = await DbContext.UserConsents.AsNoTracking().FirstOrDefaultAsync(c => c.SubjectId == subjectId && c.ClientId == clientId);
    //        return userConsent.ToModel();
    //    }

    //    public async Task RemoveUserConsentAsync(string subjectId, string clientId)
    //    {
    //        var userConsent = await DbContext.UserConsents.FirstOrDefaultAsync(c => c.SubjectId == subjectId && c.ClientId == clientId);
    //        DbContext.Remove(userConsent);
    //        await DbContext.SaveChangesAsync();
    //    }
    //}
}
