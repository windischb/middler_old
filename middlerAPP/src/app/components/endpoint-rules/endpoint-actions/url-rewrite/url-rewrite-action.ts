import { EndpointAction } from '../../models/endpoint-action';

export class UrlRewriteParameters {
    RewriteTo: string = "";
}

export class UrlRewriteAction extends EndpointAction<UrlRewriteParameters> {

    ActionType = "UrlRewrite"
    Parameters = new UrlRewriteParameters()
}
