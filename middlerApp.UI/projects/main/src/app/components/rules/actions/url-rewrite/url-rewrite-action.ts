import { MiddlerAction } from '../../models/middler-action';

export class UrlRewriteParameters {
    RewriteTo: string = "";
}

export class UrlRewriteAction extends MiddlerAction<UrlRewriteParameters> {

    ActionType = "UrlRewrite"
    Parameters = new UrlRewriteParameters()
}
