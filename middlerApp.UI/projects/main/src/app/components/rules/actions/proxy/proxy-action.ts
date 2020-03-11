import { MiddlerAction } from '../../models/middler-action';

export class ProxyParameters {
    DestinationUrl: string = "";
    UserIntermediateStream: boolean = false
    AddXForwardedHeaders: boolean = false
    CopyXForwardedHeaders: boolean = false
}

export class ProxyAction extends MiddlerAction<ProxyParameters> {

    ActionType = "Proxy"
    Parameters = new ProxyParameters()
}
