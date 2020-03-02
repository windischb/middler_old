import { MiddlerAction } from '../models/middler-action';

export class UrlRewriteParameters {
    RedirectTo: string = "";
    Permanent: boolean = false
    PreserveMethod: boolean = true
}

export class UrlRewriteAction extends MiddlerAction<UrlRewriteParameters> {

    ActionType = "UrlRewrite"
    Parameters = new UrlRewriteParameters()
}
