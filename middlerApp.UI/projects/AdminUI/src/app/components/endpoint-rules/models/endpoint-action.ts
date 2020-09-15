export class EndpointAction<T = any> {
    Id: string;
    Order: number;
    ActionType: string;
    Enabled: boolean = false;
    Parameters?: T
}


