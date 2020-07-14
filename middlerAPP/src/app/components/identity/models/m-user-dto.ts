
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

    DateTime: Date | null;

    Active: boolean;
   
    Claims: Array<any>;
    Logins: Array<any>;
    Secrets: Array<any>;

    
}

