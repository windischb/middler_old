import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { VariablesRoutingModule, RoutingComponents } from './variables-routing.module';
import { VariablesFolderContentComponent } from './folder-content/';
import { DoobCoreModule } from '@doob-ng/core';
import { DoobUIModule } from '@doob-ng/ui';
import { TreeModule } from 'angular-tree-component';
import { AngularSplitModule } from 'angular-split';
import { DoobGridModule } from "@doob-ng/grid";
import { ReactiveFormsModule } from '@angular/forms';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { VariableModalsModule } from './modals/variable-modals.module';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        VariablesRoutingModule,
        DoobCoreModule,
        DoobUIModule,
        TreeModule,
        AngularSplitModule,
        DoobGridModule,
        DoobCdkHelperModule,
        VariableModalsModule
    ],
    declarations: [
        ...RoutingComponents,
        VariablesFolderContentComponent
    ],
    exports: [
        AngularSplitModule,
    ],
    entryComponents: [

    ]
})
export class VariablesModule {

}
