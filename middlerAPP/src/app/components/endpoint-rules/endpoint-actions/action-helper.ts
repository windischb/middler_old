import { EndpointAction } from '../models/endpoint-action'


export class ActionHelper {

    static GetIcon(action: EndpointAction) {

        switch(action.ActionType) {
            case 'UrlRedirect': {
                return 'fa#directions'
            }
            case 'UrlRewrite': {
                return 'fa#pencil-alt'
            }
            case 'Proxy': {
                return 'fa#random'
            }
            case 'Script': {
                return 'fa#code'
            }
        }

        return 'hat wizard'

    }
}
