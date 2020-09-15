import { SimpleClaim } from '../claims-manager/simple-claim';
import { MRoleDto } from './m-role-dto';

export class MUserDto {
    Id: string; 
    UserName: string;
    Email: string;
    Firstname: string;
    Lastname: string;
    PhoneNumber: string;

    EmailConfirmed: boolean;
    PhoneNumberConfirmed: boolean;
    TwoFactorEnabled: boolean;
    LockoutEnabled: boolean;

    ExpiresOn: Date | null;

    Active: boolean;
   
    Claims: Array<SimpleClaim>;
    Roles: Array<MRoleDto>;

    HasPassword: boolean;
    //Logins: Array<any>;
    //Secrets: Array<any>;

    
}

