import { EndpointAction } from '../models/endpoint-action'
import { Injectable, ComponentFactoryResolver } from '@angular/core'
import { DoobModalService } from '@doob-ng/cdk-helper'
import { UrlRedirectModalComponent } from './url-redirect'
import { UrlRewriteModalComponent } from './url-rewrite'
import { ScriptModalComponent } from './script'

@Injectable({
    providedIn: 'root'
})
export class ActionHelperService {


    constructor(private modal: DoobModalService, private componentFactoryResolver: ComponentFactoryResolver) {

    }

    GetIcon(action: EndpointAction) {

        switch (action.ActionType) {
            case 'UrlRedirect': {
                return 'rollback'
            }
            case 'UrlRewrite': {
                return 'edit'
            }
            case 'Proxy': {
                return 'fa#random'
            }
            case 'Script': {
                return 'code'
            }
        }

        return 'hat wizard'

    }

    GetModal(actionType: string) {

        switch (actionType.toLowerCase()) {
            case 'urlredirect': {
                return this.modal.FromComponent(UrlRedirectModalComponent)
                    .SetModalOptions({
                        componentFactoryResolver: this.componentFactoryResolver,
                        overlayConfig: {
                            width: "50%",
                            maxWidth: "500px"
                        }
                    })
                    .CloseOnOutsideClick();
            }
            case 'urlrewrite': {
                return this.modal.FromComponent(UrlRewriteModalComponent)
                    .SetModalOptions({
                        componentFactoryResolver: this.componentFactoryResolver,
                        overlayConfig: {
                            width: "50%",
                            maxWidth: "500px"
                        }
                    })
                    .CloseOnOutsideClick();
            }
            case 'script': {
                return this.modal.FromComponent(ScriptModalComponent)
                    .SetModalOptions({
                        componentFactoryResolver: this.componentFactoryResolver,
                        overlayConfig: {
                            width: "80%",
                            height: "90%"
                        }
                    })
                    .CloseOnOutsideClick();
            }
        }
    }
}
