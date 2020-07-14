export class Secret {
    Value: string;
    Description: string;
    Expiration: Date | null;
    Type = 'SharedSecret';
}