export class MRoleDto {
    Id: string;
    Name: string;
    DisplayName: string;
    Description: string;


    /// Helper Properties
    Deleted: boolean = null
}

export function MRoleDtoSortByName(a: MRoleDto, b: MRoleDto) {
    if (a.Name?.toLowerCase() < b.Name?.toLowerCase()) {
        return -1;
    }
    if (a.Name?.toLowerCase() > b.Name?.toLowerCase()) {
        return 1;
    }
    return 0;
}