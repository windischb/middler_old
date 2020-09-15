using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace middlerApp.API.IDP.Services
{
    public class MAuthorizeInteractionResponseGenerator: AuthorizeInteractionResponseGenerator
    {
        public MAuthorizeInteractionResponseGenerator(ISystemClock clock, ILogger<AuthorizeInteractionResponseGenerator> logger, IConsentService consent, IProfileService profile) : base(clock, logger, consent, profile)
        {
        }

        protected override async Task<InteractionResponse> ProcessConsentAsync(ValidatedAuthorizeRequest request, ConsentResponse consent = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.PromptModes.Any() &&
                !request.PromptModes.Contains(OidcConstants.PromptModes.None) &&
                !request.PromptModes.Contains(OidcConstants.PromptModes.Consent))
            {
                Logger.LogError("Invalid prompt mode: {promptMode}", String.Join(' ', request.PromptModes));
                throw new ArgumentException("Invalid PromptMode");
            }

            var consentRequired = await Consent.RequiresConsentAsync(request.Subject, request.Client, request.ValidatedResources.ParsedScopes);

            if (consentRequired && request.PromptModes.Contains(OidcConstants.PromptModes.None))
            {
                Logger.LogInformation("Error: prompt=none requested, but consent is required.");

                return new InteractionResponse
                {
                    Error = OidcConstants.AuthorizeErrors.ConsentRequired
                };
            }

            if (request.PromptModes.Contains(OidcConstants.PromptModes.Consent) || consentRequired)
            {
                var response = new InteractionResponse();

                // did user provide consent
                if (consent == null)
                {
                    // user was not yet shown conset screen
                    response.IsConsent = true;
                    Logger.LogInformation("Showing consent: User has not yet consented");
                }
                else
                {
                    request.WasConsentShown = true;
                    Logger.LogTrace("Consent was shown to user");

                    // user was shown consent -- did they say yes or no
                    if (consent.Granted == false)
                    {
                        // no need to show consent screen again
                        // build error to return to client
                        Logger.LogInformation("Error: User consent result: {error}", consent.Error);

                        var error = consent.Error switch
                        {
                            AuthorizationError.AccountSelectionRequired => OidcConstants.AuthorizeErrors.AccountSelectionRequired,
                            AuthorizationError.ConsentRequired => OidcConstants.AuthorizeErrors.ConsentRequired,
                            AuthorizationError.InteractionRequired => OidcConstants.AuthorizeErrors.InteractionRequired,
                            AuthorizationError.LoginRequired => OidcConstants.AuthorizeErrors.LoginRequired,
                            _ => OidcConstants.AuthorizeErrors.AccessDenied
                        };

                        response.Error = error;
                        response.ErrorDescription = consent.ErrorDescription;
                    }
                    else
                    {
                        // double check that required scopes are in the list of consented scopes
                        var requiredScopes = request.ValidatedResources.GetRequiredScopeValues();
                        var valid = requiredScopes.All(x => consent.ScopesValuesConsented.Contains(x));
                        if (valid == false)
                        {
                            response.Error = OidcConstants.AuthorizeErrors.AccessDenied;
                            Logger.LogInformation("Error: User denied consent to required scopes");
                        }
                        else
                        {
                            // they said yes, set scopes they chose
                            request.Description = consent.Description;
                            request.ValidatedResources = request.ValidatedResources.Filter(consent.ScopesValuesConsented);
                            Logger.LogInformation("User consented to scopes: {scopes}", consent.ScopesValuesConsented);

                            if (request.Client.AllowRememberConsent && consent.RememberConsent)
                            {
                                    // remember what user actually selected
                                    var scopes = request.ValidatedResources.RawScopeValues;
                                    Logger.LogDebug("User indicated to remember consent for scopes: {scopes}", scopes);
                                    await Consent.UpdateConsentAsync(request.Subject, request.Client, request.ValidatedResources.ParsedScopes);
                                
                            }
                            
                        }
                    }
                }

                return response;
            }

            return new InteractionResponse();
        }
    }
}
