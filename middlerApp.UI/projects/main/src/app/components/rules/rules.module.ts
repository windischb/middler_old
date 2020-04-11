import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RulesRoutingModule, RoutingComponents } from './rules-routing.module';
import { OverlayModule } from '@angular/cdk/overlay';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { ActionsListComponent } from './actions-list.component';
import { ActionListItemComponent } from './action-list-item.component';
import { ActionEditModalComponent } from './modal/action-edit-modal.component';
import { ActionEditModalService } from './modal/action-modal.service';
import { UrlRedirectModalComponent } from './actions/url-redirect/url-redirect-modal.component';
import { RulesListComponent } from './rules-list.component';
import { UrlRewriteModalComponent } from './actions/url-rewrite/url-rewrite-modal.component';
import { PRoxyModalComponent } from './actions/proxy/proxy-modal.component';
import { ScriptModalComponent } from './actions/script/script-modal.component';
import { DoobCoreModule } from '@doob-ng/core';
import { DoobUIModule } from "@doob-ng/ui";
import { DoobEditorModule } from "@doob-ng/editor";

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
        ActionsListComponent,
        ActionListItemComponent,
        ActionEditModalComponent,
        UrlRedirectModalComponent,
        UrlRewriteModalComponent,
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
