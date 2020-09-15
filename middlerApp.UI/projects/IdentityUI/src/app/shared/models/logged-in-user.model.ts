import { SimpleClaim } from './simple-claim.model';

export interface LoggedInUser {
    userName: string;
    email: string;
    claims?: Array<SimpleClaim>;
    roles: Array<string>;
    firstName: string;
    lastName: string;
    fullName: string;
    isAuthenticated?: boolean;
}