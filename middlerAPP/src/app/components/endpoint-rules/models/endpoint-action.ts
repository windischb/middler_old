export class EndpointAction<T = any> {
    Id: string;
    ActionType: string;
    Enabled: boolean = false;
    Parameters?: T
}


