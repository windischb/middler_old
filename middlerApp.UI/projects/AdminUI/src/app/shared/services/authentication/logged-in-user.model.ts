import { SimpleClaim } from './simple-claim.model';

export class LoggedInUser {
    userName: string;
    email: string;
    claims?: Array<SimpleClaim>;
    roles: Array<string>;
    firstName: string;
    lastName: string;
    fullName: string;
    get isAuthenticated() {
        if(typeof this.userName == "string"){
            return !!this.userName?.trim();
        }

        var an = <any>this.userName;
        if(an instanceof Array) {
            return !!an[0]?.trim();
        }
        
    }
}