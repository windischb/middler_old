import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoutingComponents, GlobalVariablesRoutingModule } from './global-variables-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { TreeModule } from 'angular-tree-component';
import { AngularSplitModule } from 'angular-split';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { GlobalVariablesExplorerComponent } from './explorer';
import { DoobCoreModule } from '@doob-ng/core';
import { DoobGridModule } from "@doob-ng/grid";
import { VariablesFolderContentComponent } from './folder-content';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { IconsProviderModule } from 'src/app/icons-provider.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { VariableModalsModule } from './modals/variable-modals.module';
import { NzTreeModule } from 'ng-zorro-antd/tree';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        GlobalVariablesRoutingModule,
        TreeModule,
        AngularSplitModule,
        DoobCoreModule,
        DoobCdkHelperModule,
        DoobGridModule,
        NzDropDownModule,
        NzMenuModule,
        IconsProviderModule,
        FontAwesomeModule,
        VariableModalsModule,
        NzTreeModule
    ],
    declarations: [
        ...RoutingComponents,
        GlobalVariablesExplorerComponent,
        VariablesFolderContentComponent
    ],
    exports: [
        NzDropDownModule
    ]
})
export class GlobalVariablesModule { }
