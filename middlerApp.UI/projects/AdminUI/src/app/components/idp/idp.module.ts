import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { IDPRoutingModule, RoutingComponents } from './idp-routing.module';
import { ClaimsManagerComponent } from './claims-manager/claims-manager.component';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { DoobGridModule } from '@doob-ng/grid';
import { GlobalImportsModule } from '../../global-imports.module';
import { RolesManagerComponent } from './roles-manager/roles-manager.component';
import { AddRolesListComponent } from './roles-manager/add-roles-list.component';
import { UsersManagerComponent } from './users-manager/users-manager.component';
import { AddUsersListComponent } from './users-manager/add-users-list.component';
import { SecretsManagerComponent } from './secrets-manager/secrets-manager.component';
import { SecretModalComponent } from './secrets-manager/secret-modal.component';
import { ScopesManagerComponent } from './scopes-manager/scopes-manager.component';
import { ScopeGridCellComponent } from './scopes-manager/scope-grid-cell.component';
import { IconGridCellComponent } from './shared/components/icon-cell.component';

import { SetPasswordModalComponent } from './users/set-password-modal.component';
import { RoleGridCellComponent } from './roles-manager/role-grid-cell.component';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        IDPRoutingModule,
        DoobCdkHelperModule,
        DoobGridModule,
        GlobalImportsModule
       
    ],
    declarations: [
        ClaimsManagerComponent,
        RolesManagerComponent,
        AddRolesListComponent,
        UsersManagerComponent,
        AddUsersListComponent,
        SecretsManagerComponent,
        SecretModalComponent,
        ...RoutingComponents,
        ScopesManagerComponent,
        ScopeGridCellComponent,
        IconGridCellComponent,
        SetPasswordModalComponent,
        RoleGridCellComponent
    ],
    entryComponents: [
        SecretModalComponent,
        ScopeGridCellComponent,
        IconGridCellComponent,
        SetPasswordModalComponent,
        RoleGridCellComponent
    ]
})
export class IDPModule {
    
}