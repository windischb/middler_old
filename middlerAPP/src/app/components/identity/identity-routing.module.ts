import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsersComponent } from './users/users.component';
import { RolesComponent } from './roles/roles.component';
import { ClientsComponent } from './clients/clients.component';
import { UserDetailsComponent } from './users/user-details.component';
import { RoleDetailsComponent } from './roles/role-details.component';
import { ClientDetailsComponent } from './clients/client-details.component';


const routes: Routes = [
    { path: 'users', component: UsersComponent },
    { path: 'users/:id', component: UserDetailsComponent },
    { path: 'roles', component: RolesComponent },
    { path: 'roles/:id', component: RoleDetailsComponent },
    { path: 'clients', component: ClientsComponent },
    { path: 'clients/:id', component: ClientDetailsComponent }
];

export const RoutingComponents = [
    UsersComponent,
    UserDetailsComponent,
    RolesComponent,
    RoleDetailsComponent,
    ClientsComponent,
    ClientDetailsComponent
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class IdentityRoutingModule { }
