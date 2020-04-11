import { ComponentFactoryResolver, Type, Injectable } from '@angular/core';
import { OverlayConfig } from '@angular/cdk/overlay';
import { StringModalComponent } from '../modals/string-modal.component';
import { EditorModalComponent } from '../modals/editor-modal.component';
import { DoobModalService } from '@doob-ng/cdk-helper';
import { CredentialModalComponent } from '../modals/credential-modal.component';
import { NumberModalComponent } from '../modals/number-modal.component';
import { BooleanModalComponent } from '../modals/boolean-modal.component';

@Injectable({
    providedIn: 'root'
})
export class VariableModalService {


    private AvailabeModalTypes: Array<VariableModalDefinition> = [
        {
            type: EditorModalComponent,
            overlayConfig: {
                width: "80%",
                height: "90%"
            }
        }

    ]

    private ExtensionMapping: {
        [key: string]: Type<any>
    } = {
            'raw': EditorModalComponent,
            '.string': StringModalComponent,
            '.number': NumberModalComponent,
            '.boolean': BooleanModalComponent,
            '.json': EditorModalComponent,
            '.credential': CredentialModalComponent
        }


    constructor(private modal: DoobModalService, private componentFactoryResolver: ComponentFactoryResolver) {

    }

    GetModal(extension: string) {

        const modalType = this.ExtensionMapping[extension] || this.ExtensionMapping['raw'];
        const modalConf = this.AvailabeModalTypes.find(c => c.type === modalType) || {
                type: modalType,
                overlayConfig: {
                    width: '50%'
                }
            }

        return this.modal
            .FromComponent(modalConf.type)
            .SetModalOptions({
                componentFactoryResolver: this.componentFactoryResolver,
                overlayConfig: modalConf.overlayConfig
            })
            .CloseOnOutsideClick()

    }

}

class VariableModalDefinition {
    type: Type<any>;
    overlayConfig?: OverlayConfig
}
