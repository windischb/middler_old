import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { StringModalComponent } from './string/string-modal.component';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { DoobUIModule } from '@doob-ng/ui';
import { ReactiveFormsModule } from '@angular/forms';
import { DoobEditorModule } from '@doob-ng/editor';
import { EditorModalComponent } from './editor/editor-modal.component';
import { BasicModalComponent } from './base/basic-modal.component';
import { CredentialModalComponent } from './credential/credential-modal.component';
import { NumberModalComponent } from './number/number-modal.component';
import { BooleanModalComponent } from './boolean/boolean-modal.component';
import { DoobCoreModule } from '@doob-ng/core';


const ModalComponents = [
    BasicModalComponent,
    StringModalComponent,
    EditorModalComponent,
    CredentialModalComponent,
    NumberModalComponent,
    BooleanModalComponent
]

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        DoobCdkHelperModule,
        DoobUIModule,
        DoobEditorModule,
        DoobCoreModule

    ],
    declarations: [
        ...ModalComponents
    ],
    entryComponents:[
        ...ModalComponents
    ],
    exports: [
        ...ModalComponents
    ]
})
export class VariableModalsModule {

}
