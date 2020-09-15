export class MUserListDto {
    Id: string;
    UserName: string;
    Email: string;
    Firstname: string;
    Lastname: string;

    EmailConfirmed: boolean;
    Active: boolean;
    
    HasPassword: boolean;
    Logins: Array<string>;
    Deleted: boolean;
}



export function MUserListDtoSortByName(a: MUserListDto, b: MUserListDto) {
    if (a.UserName?.toLowerCase() < b.UserName?.toLowerCase()) {
        return -1;
    }
    if (a.UserName?.toLowerCase() > b.UserName?.toLowerCase()) {
        return 1;
    }
    return 0;
}