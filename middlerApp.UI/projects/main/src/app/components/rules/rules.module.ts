import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RulesRoutingModule, RoutingComponents } from './rules-routing.module';
import { FuiTabsModule } from "@fui/tabs";
import { FuiDropdownModule } from "@fui/dropdown";
import { FuiCoreModule } from "@fui/core";
import { OverlayModule } from '@angular/cdk/overlay';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { ActionsListComponent } from './actions-list.component';
import { ActionListItemComponent } from './action-list-item.component';
import { ActionEditModalComponent } from './modal/action-edit-modal.component';
import { ActionEditModalService } from './modal/action-modal.service';
import { UrlRedirectModalComponent } from './actions/url-redirect/url-redirect-modal.component';
import { RulesListComponent } from './rules-list.component';
import { FuiCheckboxModule } from "@fui/checkbox";
import { UrlRewriteModalComponent } from './actions/url-rewrite/url-rewrite-modal.component';
import { PRoxyModalComponent } from './actions/proxy/proxy-modal.component';
import { ScriptModalComponent } from './actions/script/script-modal.component';
import { FuiEditorModule } from "@fui/editor";
@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        RulesRoutingModule,
        FuiCoreModule,
        FuiTabsModule,
        FuiDropdownModule,
        OverlayModule,
        DragDropModule,
        FuiCheckboxModule,
        FuiEditorModule
    ],
    declarations: [
        ...RoutingComponents,
        ActionsListComponent,
        ActionListItemComponent,
        ActionEditModalComponent,
        UrlRedirectModalComponent,
        UrlRewriteModalComponent,
        RulesListComponent,
        PRoxyModalComponent,
        ScriptModalComponent
    ],
    entryComponents: [
        ActionEditModalComponent,
        UrlRedirectModalComponent,
        UrlRewriteModalComponent,
        PRoxyModalComponent,
        ScriptModalComponent
    ],
    providers: [
        ActionEditModalService
    ],
    exports: []
})
export class RulesModule { }
