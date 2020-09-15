import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EndpointRulesRoutingModule, RoutingComponents } from './endpoint-rules-routing.module';
import { EndpointRulesListComponent } from './endpoint-rules-list/endpoint-rules-list.component';
import { DragDropModule } from "@angular/cdk/drag-drop";

import { IconsProviderModule } from 'src/app/icons-provider.module';

import { ReactiveFormsModule } from '@angular/forms';

import { EndpointActionsListComponent } from './endpoint-actions-list/endpoint-actions-list.component';
import { ActionListDetailsComponent } from './endpoint-actions-list/action-list-details/action-list-details.component';
import { ActionBasicModalComponent } from './endpoint-actions/base/action-basic-modal.component';
import { UrlRewriteModalComponent, UrlRedirectModalComponent, ProxyModalComponent, ScriptModalComponent } from './endpoint-actions';

import { GlobalImportsModule } from '../../global-imports.module';



const ActionComponents = [
    ActionBasicModalComponent,
    UrlRewriteModalComponent, UrlRedirectModalComponent, ProxyModalComponent, ScriptModalComponent
]

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        EndpointRulesRoutingModule,
        DragDropModule,
        IconsProviderModule,
        GlobalImportsModule
    ],
    declarations: [
        ...RoutingComponents,
        ...ActionComponents,
        EndpointRulesListComponent,
        EndpointActionsListComponent,
        ActionListDetailsComponent
    ],
    entryComponents: [
        ...ActionComponents,
    ]
    
})
export class EndpointRulesModule { }
