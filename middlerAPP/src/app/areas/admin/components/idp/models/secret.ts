export class Secret {
    Id: string;
    Value: string;
    Description: string;
    Expiration: Date | null;
    Type = 'SharedSecret';
}

export function SecretSortByExpiration(a: Secret, b: Secret) {
    if (a.Expiration < b.Expiration) {
        return -1;
    }
    if (a.Expiration > b.Expiration) {
        return 1;
    }
    return 0;
}