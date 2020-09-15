import { ComponentFactoryResolver, Type, Injectable } from '@angular/core';
import { OverlayConfig } from '@angular/cdk/overlay';
import { EditorModalComponent, StringModalComponent, CredentialModalComponent, NumberModalComponent, BooleanModalComponent } from '../modals';
import { DoobModalService } from '@doob-ng/cdk-helper';

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
            'string': StringModalComponent,
            'number': NumberModalComponent,
            'boolean': BooleanModalComponent,
            'json': EditorModalComponent,
            'credential': CredentialModalComponent
        }


    constructor(private modal: DoobModalService, private componentFactoryResolver: ComponentFactoryResolver) {

    }

    GetModal(extension: string) {

        const modalType = this.ExtensionMapping[extension] || this.ExtensionMapping['raw'];
        const modalConf = this.AvailabeModalTypes.find(c => c.type === modalType) || {
                type: modalType,
                overlayConfig: {
                    width: '50%',
                    maxWidth: '500px'
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
