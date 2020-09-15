using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;

namespace middlerApp.API.Controllers.IdP.Account.ViewModels
{
    public class LoginResultModel
    {
        public string ReturnUrl { get; set; }
        public Status Status { get; set; }
        public List<LoginError> Errors { get; set; } = new List<LoginError>();
        public string ProviderName { get; set; }
        public LoginResultUserModel UserData { get; set; } = new LoginResultUserModel();
        public bool RememberLogin { get; set; }

        public LoginResultModel WithStatus(Status status)
        {
            this.Status = status;
            return this;
        }

        public LoginResultModel WithError(string message, object data = null)
        {
            var err = new LoginError();
            err.Message = message;
            //err.Data = data?.ToDotNetDictionary();
            return WithError(err);
        }

        public LoginResultModel WithError(LoginError error)
        {
            if (Errors == null)
            {
                Errors = new List<LoginError>();
            }
            Errors.Add(error);
            return this;
        }

        public LoginResultModel WithError(IEnumerable<LoginError> errors)
        {
            foreach (var loginError in errors)
            {
                WithError(loginError);
            }
            return this;
        }

    }

    public enum Status
    {
        Error,
        MustConfirm,
        Confirmed,
        Ok,
        IsLockedOut,
        RequiresTwoFactor
    }

    public class LoginResultUserModel
    {
        public string Identifier { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool RememberLogin { get; set; }


        public ClaimsPrincipal UpdateClaimsPrincipal(ClaimsPrincipal claimsPrincipal, string provider)
        {

            var claims = new List<Claim>();
            foreach (var claimsPrincipalClaim in claimsPrincipal.Claims)
            {
                if (!claimsPrincipalClaim.Type.Equals("name"))
                    claims.Add(new Claim(claimsPrincipalClaim.Type, claimsPrincipalClaim.Value, claimsPrincipalClaim.ValueType, provider));
            }

            if (!String.IsNullOrWhiteSpace(FirstName))
                claims.Add(new Claim(JwtClaimTypes.GivenName, FirstName, "string", provider));

            if (!String.IsNullOrWhiteSpace(LastName))
                claims.Add(new Claim(JwtClaimTypes.FamilyName, LastName, "string", provider));

            if (!String.IsNullOrWhiteSpace(Email))
                claims.Add(new Claim(JwtClaimTypes.Email, Email, "string", provider));

            if (!String.IsNullOrWhiteSpace(PhoneNumber))
                claims.Add(new Claim(JwtClaimTypes.PhoneNumber, PhoneNumber, "string", provider));

            var ci = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(ci);
        }
    }
}
