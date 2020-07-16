import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { IdentityRoutingModule, RoutingComponents } from './identity-routing.module';
import { ClaimsManagerComponent } from './claims-manager/claims-manager.component';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';
import { DoobGridModule } from '@doob-ng/grid';
import { GlobalModules, GlobalImportsModule } from 'src/app/global-imports.module';
import { RolesManagerComponent } from './roles-manager/roles-manager.component';
import { AddRolesListComponent } from './roles-manager/add-roles-list.component';
import { UsersManagerComponent } from './users-manager/users-manager.component';
import { AddUsersListComponent } from './users-manager/add-users-list.component';


@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        IdentityRoutingModule,
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
        ...RoutingComponents
    ]
})
export class IdentityModule {
    
}