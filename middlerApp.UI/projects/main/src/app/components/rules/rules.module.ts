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
import { UrlRedirectModalComponent } from './actions/url-redirect-modal.component';
import { RulesListComponent } from './rules-list.component';

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
        DragDropModule
    ],
    declarations: [
        ...RoutingComponents,
        ActionsListComponent,
        ActionListItemComponent,
        ActionEditModalComponent,
        UrlRedirectModalComponent,
        RulesListComponent
    ],
    entryComponents: [
        ActionEditModalComponent,
        UrlRedirectModalComponent
    ],
    providers: [
        ActionEditModalService
    ],
    exports: []
})
export class RulesModule { }
