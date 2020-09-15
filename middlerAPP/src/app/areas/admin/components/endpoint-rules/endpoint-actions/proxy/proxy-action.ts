import { EndpointAction } from '../../models/endpoint-action';

export class ProxyParameters {
    DestinationUrl: string = "";
    UserIntermediateStream: boolean = false
    AddXForwardedHeaders: boolean = false
    CopyXForwardedHeaders: boolean = false
}

export class ProxyAction extends EndpointAction<ProxyParameters> {

    ActionType = "Proxy"
    Parameters = new ProxyParameters()
}
