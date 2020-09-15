using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace middlerApp.API.IDP.Services
{
    public class LocalUserProfileService : IProfileService
    {
        private readonly ILocalUserService _localUserService;

        public LocalUserProfileService(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = (await _localUserService.GetUserClaimsBySubjectAsync(subjectId))
                .ToList();

            var user = await _localUserService.GetUserBySubjectAsync(subjectId);

            var claims = new List<Claim>();

            if (!String.IsNullOrEmpty(user.FirstName))
            {
                claims.Add(new Claim("given_name", user.FirstName));
            }

            if (!String.IsNullOrEmpty(user.LastName))
            {
                claims.Add(new Claim("family_name", user.LastName));
            }

            if (!String.IsNullOrEmpty(user.UserName))
            {
                claims.Add(new Claim("name", user.UserName));
            }

            if (!String.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim("email", user.Email));
            }

            if (context.RequestedClaimTypes.Contains("role"))
            {
                foreach (var userUserRole in user.UserRoles)
                {
                    claims.Add(new Claim("role", userUserRole.Role.Name));
                }
            }
            


            context.AddRequestedClaims(claims);
            context.AddRequestedClaims(
                claimsForUser.Select(c => new Claim(c.Type, c.Value)).ToList());
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            context.IsActive = await _localUserService.IsUserActive(subjectId);
        }
    }
}
