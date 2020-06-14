export class EndpointRuleListDto {
    Id: string;
    Name: string;
    Scheme: Array<string>;
    Hostname: string;
    Path: string;
    HttpMethods: Array<string>;
    Actions: Array<string>;
    Order: number;
    Enabled: boolean;
}
