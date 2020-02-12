export class MiddlerRulePermission {
    PrincipalName?: string;
    Type: PermissionType;
    AccessMode: AccessMode;
    Client?: string;
    SourceAddress?: string;
}

export type PermissionType = "User" | "Role" | "Authenticated" | "Everyone";
export type AccessMode = "Allow" | "Deny" | "Ignore";
