import { EndpointRulePermission } from './endpoint-rule-permission';
import { EndpointAction } from './endpoint-action';

export class EndpointRule {
    Id: string;
    Name: string;
    Scheme: Array<string>;
    Hostname: string;
    Path: string;
    HttpMethods: Array<string>;
    Permissions: Array<EndpointRulePermission>;
    Actions: Array<EndpointAction>;
    Order: number;
    Enabled: boolean;
}
