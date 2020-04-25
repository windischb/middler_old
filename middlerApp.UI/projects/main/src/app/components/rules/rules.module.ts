import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RulesRoutingModule, RoutingComponents } from './rules-routing.module';
import { OverlayModule } from '@angular/cdk/overlay';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { DoobCoreModule } from '@doob-ng/core';
import { DoobUIModule } from "@doob-ng/ui";
import { DoobEditorModule } from "@doob-ng/editor";
import { ActionsListComponent } from './rule-details/actions-list/actions-list.component';
import { ActionListItemComponent } from './rule-details/action-list-item/action-list-item.component';

import { UrlRewriteModalComponent, UrlRedirectModalComponent, ProxyModalComponent, ScriptModalComponent } from './actions';
import { ActionBasicModalComponent } from './actions/base/action-basic-modal.component';

const ActionComponents = [
    ActionBasicModalComponent,
    UrlRewriteModalComponent, UrlRedirectModalComponent, ProxyModalComponent, ScriptModalComponent
]
@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        RulesRoutingModule,
        DoobCoreModule,
        OverlayModule,
        DragDropModule,
        DoobUIModule,
        DoobEditorModule,
        DoobCoreModule
    ],
    declarations: [
        ...RoutingComponents,
        ...ActionComponents,
        ActionsListComponent,
        ActionListItemComponent
    ],
    entryComponents: [
        ...ActionComponents,

    ],
    providers: [

    ],
    exports: []
})
export class RulesModule { }
