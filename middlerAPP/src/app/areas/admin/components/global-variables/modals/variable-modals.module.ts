import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { StringModalComponent } from './string/string-modal.component';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { ReactiveFormsModule } from '@angular/forms';
import { DoobEditorModule } from '@doob-ng/editor';
import { EditorModalComponent } from './editor/editor-modal.component';
import { BasicModalComponent } from './base/basic-modal.component';
import { CredentialModalComponent } from './credential/credential-modal.component';
import { NumberModalComponent } from './number/number-modal.component';
import { BooleanModalComponent } from './boolean/boolean-modal.component';
import { DoobCoreModule } from '@doob-ng/core';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzSelectModule } from 'ng-zorro-antd/select';

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
        DoobEditorModule,
        DoobCoreModule,
        NzFormModule,
        NzInputModule,
        NzButtonModule,
        NzInputNumberModule,
        NzCheckboxModule,
        NzSelectModule
    ],
    declarations: [
        ...ModalComponents
    ],
    entryComponents: [
        ...ModalComponents
    ],
    exports: [
        ...ModalComponents
    ]
})
export class VariableModalsModule {

}
