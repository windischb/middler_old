
export interface IMIdentityResourceDto {
    Id: string;
    Enabled: boolean;
    Name: string;
    DisplayName: string;
    Description: string;
    Required: boolean;
    Emphasize: boolean;
    ShowInDiscoveryDocument: boolean;
    UserClaims: Array<string>;
    Properties: { [key: string]: string }
    Created: Date;
    Updated: Date | null;
    NonEditable: boolean;
}