import { MiddlerAction } from '../models/middler-action';

export class ActionHelper {

    static GetIcon(action: MiddlerAction) {

        switch(action.ActionType) {
            case 'UrlRedirect': {
                return 'directions'
            }
            case 'UrlRewrite': {
                return 'pencil alternate'
            }
            case 'Proxy': {
                return 'random'
            }
            case 'Script': {
                return 'code'
            }
        }

        return 'hat wizard'

    }
}
