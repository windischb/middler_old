import { MiddlerRulePermission } from './middler-rule-permission';
import { MiddlerAction } from './middler-action';

export class CreateMiddlerRuleDto {
    Name: string;
    Scheme: Array<string>;
    Hostname: string;
    Path: string;
    HttpMethods: Array<string>;
    Permissions: Array<MiddlerRulePermission>;
    Actions: Array<MiddlerAction>;
    Enabled: boolean;
    Order: number;
}
