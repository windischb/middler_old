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
import { IdentityClientsStore, IdentityClientsQuery } from './clients/identity-clients.store';
import { ApiResourcesStore, ApiResourcesQuery } from './api-resources/api-resources.store';
import { IMApiResourceListDto } from './models/m-api-resource-list-dto';
import { IMApiResourceDto } from './models/m-api-resource-dto';
import { IMIdentityResourceListDto } from './models/m-identity-resource-list-dto';
import { IdentityResourcesStore, IdentityResourcesQuery } from './identity-resources/identity-resources.store';
import { IMIdentityResourceDto } from './models/m-identity-resource-dto';

@Injectable({
    providedIn: 'root'
})
export class IDPService {


    constructor(
        private identityRolesStore: IdentityRolesStore,
        private identityRolesQuery: IdentityRolesQuery,
        private identityUsersStore: IdentityUsersStore,
        private identityUsersQuery: IdentityUsersQuery,
        private identityClientsStore: IdentityClientsStore,
        private identityClientsQuery: IdentityClientsQuery,
        private identityApiResourcesStore: ApiResourcesStore,
        private identityApiResourcesQuery: ApiResourcesQuery,
        private identityResourcesStore: IdentityResourcesStore,
        private identityResourcesQuery: IdentityResourcesQuery,
        private http: HttpClient,
        private message: MessageService) {

        this.message.RunOnEveryReconnect(() => {
            this.SubscribeUserEvents();
            this.SubscribeRoleEvents();
            this.SubscribeClientsEvents();
            this.SubscribeApiResourcesEvents();
            this.SubscribeIdentityResourcesEvents();
        });

    }

    //#region  Users
    SubscribeUserEvents() {
        this.message.Stream<DataEvent<MUserListDto>>("IDPUsers.Subscribe").pipe(
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
        this.http.get<Array<MUserListDto>>(`api/idp/users`).pipe(
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
        return this.http.get<MUserDto>(`api/idp/users/${id}`)
    }

    CreateUser(createUserModel: MUserDto) {
        return this.http.post(`api/idp/users`, createUserModel);
    }

    UpdateUser(createUserModel: MUserDto) {
        return this.http.put(`api/idp/users`, createUserModel);
    }

    DeleteUser(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/idp/users`, options);
    }

    //#endregion

    //#region Roles

    SubscribeRoleEvents() {
        this.message.Stream<DataEvent<MRoleDto>>("IDPRoles.Subscribe").pipe(
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
        this.message.Invoke<Array<MRoleDto>>("IDPRoles.GetRolesList").pipe(
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
        return this.http.get<MRoleDto>(`api/idp/roles/${id}`)
    }

    CreateRole(roleModel: MRoleDto) {
        return this.http.post(`api/idp/roles`, roleModel);
    }

    UpdateRole(roleModel: MRoleDto) {
        return this.http.put(`api/idp/roles`, roleModel);
    }

    DeleteRole(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/idp/roles`, options);
    }
    //#endregion

    //#region Clients

    SubscribeClientsEvents() {
        this.message.Stream<DataEvent<IMClientDto>>("IDPClients.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityClientsStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityClientsStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityClientsStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                    }
                }

            })
        ).subscribe()
    }
    ReLoadClients() {
        this.http.get<Array<IMClientDto>>(`api/idp/clients`).pipe(
            tap(users => this.identityClientsStore.set(users))
        ).subscribe();
    }

    GetAllClients(force?: boolean) {
        if (force || !this.identityClientsQuery.getHasCache()) {
            this.ReLoadClients()
        }

        return this.identityClientsQuery.selectAll();
    }

    GetClient(id: string) {
        return this.http.get<IMClientDto>(`api/idp/clients/${id}`)
    }

    CreateClient(dtoModel: IMClientDto) {
        return this.http.post(`api/idp/clients`, dtoModel);
    }

    UpdateClient(dtoModel: IMClientDto) {
        return this.http.put(`api/idp/clients`, dtoModel);
    }

    DeleteClient(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/idp/clients`, options);
    }

    //#endregion

    //#region ApiResources

    SubscribeApiResourcesEvents() {
        this.message.Stream<DataEvent<IMApiResourceListDto>>("IDPApiResources.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityApiResourcesStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityApiResourcesStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityApiResourcesStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                    }
                }

            })
        ).subscribe()
    }
    ReLoadApiResources() {
        this.http.get<Array<IMApiResourceListDto>>(`api/idp/api-resources`).pipe(
            tap(users => this.identityApiResourcesStore.set(users))
        ).subscribe();
    }

    GetAllApiResources(force?: boolean) {
        if (force || !this.identityApiResourcesQuery.getHasCache()) {
            this.ReLoadApiResources()
        }

        return this.identityApiResourcesQuery.selectAll();
    }

    GetApiResource(id: string) {
        return this.http.get<IMApiResourceDto>(`api/idp/api-resources/${id}`)
    }

    CreateApiResource(dtoModel: IMApiResourceDto) {
        return this.http.post(`api/idp/api-resources`, dtoModel);
    }

    UpdateApiResource(dtoModel: IMApiResourceDto) {
        return this.http.put(`api/idp/api-resources`, dtoModel);
    }

    DeleteApiResource(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/idp/api-resources`, options);
    }

    //#endregion

    //#region IdentityResources

    SubscribeIdentityResourcesEvents() {
        this.message.Stream<DataEvent<IMIdentityResourceListDto>>("IDPIdentityResources.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityResourcesStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityResourcesStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityResourcesStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                    }
                }

            })
        ).subscribe()
    }
    ReLoadIdentityResources() {
        this.http.get<Array<IMIdentityResourceListDto>>(`api/idp/identity-resources`).pipe(
            tap(users => this.identityResourcesStore.set(users))
        ).subscribe();
    }

    GetAllIdentityResources(force?: boolean) {
        if (force || !this.identityResourcesQuery.getHasCache()) {
            this.ReLoadIdentityResources()
        }

        return this.identityResourcesQuery.selectAll();
    }

    GetIdentityResource(id: string) {
        return this.http.get<IMIdentityResourceDto>(`api/idp/identity-resources/${id}`)
    }

    CreateIdentityResource(dtoModel: IMIdentityResourceDto) {
        return this.http.post(`api/idp/identity-resources`, dtoModel);
    }

    UpdateIdentityResource(dtoModel: IMIdentityResourceDto) {
        return this.http.put(`api/idp/identity-resources`, dtoModel);
    }

    DeleteIdentityResource(...ids: string[]) {

        const options = {
            body: ids
        }

        return this.http.request("delete", `api/idp/identity-resources`, options);
    }

    //#endregion
}