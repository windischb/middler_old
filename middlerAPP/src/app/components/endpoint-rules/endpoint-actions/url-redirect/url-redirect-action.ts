import { EndpointAction } from '../../models/endpoint-action';


export class UrlRedirectParameters {
    RedirectTo: string = "";
    Permanent: boolean = false
    PreserveMethod: boolean = true
}

export class UrlRedirectAction extends EndpointAction<UrlRedirectParameters> {

    ActionType = "UrlRedirect"
    Parameters = new UrlRedirectParameters()
}
