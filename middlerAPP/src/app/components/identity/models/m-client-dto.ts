
import { SimpleClaim } from '../claims-manager/simple-claim';
import { Secret } from './secret';

export interface IMClientDto {
    Id: string;
    Enabled: boolean;
    ClientId: string;
    ClientName: string;
    ProtocolType: string;
    ClientSecrets: Array<Secret>;
    RequireClientSecret: boolean;
    Description: string;
    ClientUri: string;
    LogoUri: string;
    RequireConsent: boolean;
    AllowRememberConsent: boolean;
    AlwaysIncludeUserClaimsInIdToken: boolean;
    AllowedGrantTypes: Array<string>;
    RequirePkce: boolean;
    AllowPlainTextPkce: boolean;
    RequireRequestObject: boolean;
    AllowAccessTokensViaBrowser: boolean;
    RedirectUris: Array<string>;
    PostLogoutRedirectUris: Array<string>;
    FrontChannelLogoutUri: string;
    FrontChannelLogoutSessionRequired: boolean;
    BackChannelLogoutUri: string;
    BackChannelLogoutSessionRequired: boolean;
    AllowOfflineAccess: boolean;
    AllowedScopes: Array<string>;
    IdentityTokenLifetime: number;
    AllowedIdentityTokenSigningAlgorithms: string;
    AccessTokenLifetime: number;
    AuthorizationCodeLifetime: number;
    ConsentLifetime: number | null;
    AbsoluteRefreshTokenLifetime: number;
    SlidingRefreshTokenLifetime: number;
    RefreshTokenUsage: number;
    UpdateAccessTokenClaimsOnRefresh: boolean;
    RefreshTokenExpiration: number;
    AccessTokenType: number;
    EnableLocalLogin: boolean;
    IdentityProviderRestrictions: Array<string>;
    IncludeJwtId: boolean;
    Claims: Array<SimpleClaim>
    AlwaysSendClientClaims: boolean;
    ClientClaimsPrefix: string;
    PairWiseSubjectSalt: string;
    AllowedCorsOrigins: Array<string>;
    Properties: { [key: string]: string }
    Created: Date;
    Updated: Date | null;
    LastAccessed: Date | null;
    UserSsoLifetime: number | null;
    UserCodeType: string;
    DeviceCodeLifetime: number;
    NonEditable: boolean;
}