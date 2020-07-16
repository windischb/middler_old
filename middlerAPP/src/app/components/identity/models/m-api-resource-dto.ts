import { Secret } from './secret';

export interface IMApiResourceDto {
    Id: string;
    Enabled: boolean;
    Name: string;
    DisplayName: string;
    Description: string;
    AllowedAccessTokenSigningAlgorithms: string;
    ShowInDiscoveryDocument: boolean;
    Secrets: Array<Secret>;
    Scopes: Array<string>;
    UserClaims: Array<string>;
    Properties: { [key: string]: string }
    Created: Date;
    Updated: Date | null;
    LastAccessed: Date | null;
    NonEditable: boolean;
}