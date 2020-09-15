export interface IMScopeDto {
    Id: string;
    Enabled: boolean;
    Name: string;
    DisplayName: string;
    Description: string;
    Required: boolean;
    Emphasize: boolean;
    ShowInDiscoveryDocument: boolean;
    Type: string;
    UserClaims: Array<string>;
    Properties: { [key: string]: string }
}