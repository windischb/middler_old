import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { MessageService } from '@services';
import { MUserDto } from './models/m-user-dto';
import { of } from 'rxjs';
import { MRoleDto } from './models/m-role-dto';
import { IMClientDto } from './models/m-client-dto';
import { IdentityRolesStore, IdentityRolesQuery } from 'src/app/identity-roles.store';
import { tap } from 'rxjs/operators';
import { IdentityUsersQuery, IdentityUsersStore } from 'src/app/identity-users.store';
import { DataEvent } from 'src/app/shared/models/data-event';
import { MUserListDto } from './models/m-user-list-dto';

@Injectable({
    providedIn: 'root'
})
export class IdentityService {


    constructor(
        private identityRolesStore: IdentityRolesStore,
        private identityRolesQuery: IdentityRolesQuery,
        private identityUsersStore: IdentityUsersStore,
        private identityUsersQuery: IdentityUsersQuery,
        private http: HttpClient,
        private message: MessageService) {

        this.message.RunOnEveryReconnect(() => {
            this.SubscribeUserEvents();
        });
        this.message.RunOnEveryReconnect(() => {
            this.SubscribeRoleEvents();
        });


    }

    //#region  Users
    SubscribeUserEvents() {
        this.message.Stream<DataEvent<MUserListDto>>("IdentityUsers.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityUsersStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityUsersStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityUsersStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                    }
                }

            })
        ).subscribe()
    }
    ReLoadUsers() {
        this.http.get<Array<MUserDto>>(`api/identity/users`).pipe(
            tap(users => this.identityUsersStore.set(users))
        ).subscribe();
    }
    GetAllUsers(force?: boolean) {
        if (force || !this.identityUsersQuery.getHasCache()) {
            this.ReLoadUsers()
        }

        return this.identityUsersQuery.selectAll();
    }

    GetUser(id: string) {
        return this.http.get<MUserDto>(`api/identity/users/${id}`)
    }

    CreateUser(createUserModel: MUserDto) {
        return this.http.post(`api/identity/users`, createUserModel);
    }

    UpdateUser(createUserModel: MUserDto) {
        return this.http.put(`api/identity/users`, createUserModel);
    }

    DeleteUser(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/identity/users`, options);
    }

    //#endregion

    //#region Roles

    SubscribeRoleEvents() {
        this.message.Stream<DataEvent<MRoleDto>>("IdentityRoles.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityRolesStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityRolesStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityRolesStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                        //this.identityRolesStore.remove(<any>item.Payload)
                    }
                }

            })
        ).subscribe()
    }
    ReLoadRoles() {
        this.message.Invoke<Array<MRoleDto>>("IdentityRoles.GetRolesList").pipe(
            tap(roles => this.identityRolesStore.set(roles))
        ).subscribe();
    }
    GetAllRoles(force?: boolean) {
        if (force || !this.identityRolesQuery.getHasCache()) {
            this.ReLoadRoles()
        }

        return this.identityRolesQuery.selectAll();
    }

    GetRole(id: string) {
        return this.http.get<MRoleDto>(`api/identity/roles/${id}`)
    }

    CreateRole(roleModel: MRoleDto) {
        return this.http.post(`api/identity/roles`, roleModel);
    }

    UpdateRole(roleModel: MRoleDto) {
        return this.http.put(`api/identity/roles`, roleModel);
    }

    DeleteRole(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/identity/roles`, options);
    }
    //#endregion

    //#region Clients
    GetAllClients() {
        return this.http.get<Array<IMClientDto>>(`api/identity/clients`)
    }
    GetClient(id: string) {
        return this.http.get<IMClientDto>(`api/identity/clients/${id}`)
    }

    CreateClient(clientModel: IMClientDto) {
        return this.http.post(`api/identity/clients`, clientModel);
    }

    UpdateClient(clientModel: IMClientDto) {
        return this.http.put(`api/identity/clients`, clientModel);
    }
    //#endregion
}