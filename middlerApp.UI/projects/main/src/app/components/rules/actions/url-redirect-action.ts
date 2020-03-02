import { MiddlerAction } from '../models/middler-action';

export class UrlRedirectParameters {
    RedirectTo: string = "";
    Permanent: boolean = false
    PreserveMethod: boolean = true
}

export class UrlRedirectAction extends MiddlerAction<UrlRedirectParameters> {

    ActionType = "UrlRedirect"
    Parameters = new UrlRedirectParameters()
}
