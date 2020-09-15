import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MessageService } from '@services';
import { MUserDto } from './models/m-user-dto';
import { of } from 'rxjs';
import { MRoleDto } from './models/m-role-dto';
import { IMClientDto } from './models/m-client-dto';
import { IdentityRolesStore, IdentityRolesQuery } from './identity-roles.store';
import { tap, subscribeOn } from 'rxjs/operators';
import { IdentityUsersQuery, IdentityUsersStore } from './identity-users.store';
import { DataEvent } from '../../shared/models/data-event';
import { MUserListDto } from './models/m-user-list-dto';
import { IdentityClientsStore, IdentityClientsQuery } from './clients/identity-clients.store';
import { ApiResourcesStore, ApiResourcesQuery } from './api-resources/api-resources.store';
import { IMApiResourceListDto } from './models/m-api-resource-list-dto';
import { IMApiResourceDto } from './models/m-api-resource-dto';
import { IMIdentityResourceListDto } from './models/m-identity-resource-list-dto';
import { IdentityResourcesStore, IdentityResourcesQuery } from './identity-resources/identity-resources.store';
import { IMIdentityResourceDto } from './models/m-identity-resource-dto';
import { ApiScopesStore, ApiScopesQuery } from './api-scopes/api-scopes.store';
import { IMScopeDto } from './models/m-scope-dto';
import { SetPasswordDto } from './models/set-password-dto';
import { OAuthService } from 'angular-oauth2-oidc';

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
        private identityApiScopesStore: ApiScopesStore,
        private identityApiScopesQuery: ApiScopesQuery,
        private identityResourcesStore: IdentityResourcesStore,
        private identityResourcesQuery: IdentityResourcesQuery,
        private http: HttpClient,
        private message: MessageService,
        private auth: OAuthService) {

        this.message.RunOnEveryReconnect(() => {
           this.SubscribeToAll();
            
        });
        this.SubscribeToAll();
    }

    SubscribeToAll() {
        this.SubscribeUserEvents();
        this.SubscribeRoleEvents();
        this.SubscribeClientsEvents();
        this.SubscribeApiResourcesEvents();
        this.SubscribeIdentityResourcesEvents();
    }

    getHeaders() {
        return new HttpHeaders({
            'Authorization': 'Bearer ' + this.auth.getAccessToken()
        });
    }

    // HttpGet<T>(url: string) {


    //     return this.http.get<T>(url, { headers: this.getHeaders() });
    // }

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
        this.http.get<Array<MUserListDto>>(`api/idp/users`, { headers: this.getHeaders() }).pipe(
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
        return this.http.get<MUserDto>(`api/idp/users/${id}`, { headers: this.getHeaders() })
    }

    CreateUser(createUserModel: MUserDto) {
        return this.http.post(`api/idp/users`, createUserModel, { headers: this.getHeaders() });
    }

    UpdateUser(createUserModel: MUserDto) {
        return this.http.put(`api/idp/users`, createUserModel, { headers: this.getHeaders() });
    }

    DeleteUser(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
        }

        return this.http.request("delete", `api/idp/users`, options);
    }


    SetPassword(userId: string, dto: SetPasswordDto) {
        return this.http.post(`api/idp/users/${userId}/password`, dto, { headers: this.getHeaders() });
    }

    ClearPassword(userId: string) {
        return this.http.delete(`api/idp/users/${userId}/password`, { headers: this.getHeaders() });
    }

    //#endregion

    //#region Roles

    SubscribeRoleEvents() {
        console.log("IDPRoles.Subscribe")
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
        return this.http.get<MRoleDto>(`api/idp/roles/${id}`, { headers: this.getHeaders() })
    }

    CreateRole(roleModel: MRoleDto) {
        return this.http.post(`api/idp/roles`, roleModel, { headers: this.getHeaders() });
    }

    UpdateRole(roleModel: MRoleDto) {
        return this.http.put(`api/idp/roles`, roleModel, { headers: this.getHeaders() });
    }

    DeleteRole(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
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
        this.http.get<Array<IMClientDto>>(`api/idp/clients`, { headers: this.getHeaders() }).pipe(
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
        return this.http.get<IMClientDto>(`api/idp/clients/${id}`, { headers: this.getHeaders() })
    }

    CreateClient(dtoModel: IMClientDto) {
        return this.http.post(`api/idp/clients`, dtoModel, { headers: this.getHeaders() });
    }

    UpdateClient(dtoModel: IMClientDto) {
        return this.http.put(`api/idp/clients`, dtoModel, { headers: this.getHeaders() });
    }

    DeleteClient(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
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
        this.http.get<Array<IMApiResourceListDto>>(`api/idp/api-resources`, { headers: this.getHeaders() }).pipe(
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
        return this.http.get<IMApiResourceDto>(`api/idp/api-resources/${id}`, { headers: this.getHeaders() })
    }

    CreateApiResource(dtoModel: IMApiResourceDto) {
        return this.http.post(`api/idp/api-resources`, dtoModel, { headers: this.getHeaders() });
    }

    UpdateApiResource(dtoModel: IMApiResourceDto) {
        return this.http.put(`api/idp/api-resources`, dtoModel, { headers: this.getHeaders() });
    }

    DeleteApiResource(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
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
        this.http.get<Array<IMIdentityResourceListDto>>(`api/idp/identity-resources`, { headers: this.getHeaders() }).pipe(
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
        return this.http.get<IMIdentityResourceDto>(`api/idp/identity-resources/${id}`, { headers: this.getHeaders() })
    }

    CreateIdentityResource(dtoModel: IMIdentityResourceDto) {
        return this.http.post(`api/idp/identity-resources`, dtoModel, { headers: this.getHeaders() });
    }

    UpdateIdentityResource(dtoModel: IMIdentityResourceDto) {
        return this.http.put(`api/idp/identity-resources`, dtoModel, { headers: this.getHeaders() });
    }

    DeleteIdentityResource(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
        }

        return this.http.request("delete", `api/idp/identity-resources`, options);
    }

    //#endregion


    //#region ApiScopes

    SubscribeApiScopesEvents() {
        this.message.Stream<DataEvent<IMScopeDto>>("IDPApiScopes.Subscribe").pipe(
            tap(item => {
                console.log(item)
                switch (item.Action) {
                    case "Created": {
                        this.identityApiScopesStore.add(item.Payload);
                        break;
                    }
                    case "Updated": {
                        this.identityApiScopesStore.update(item.Payload.Id, entity => item.Payload);
                        break;
                    }
                    case "Deleted": {
                        this.identityApiScopesStore.update(<any>item.Payload, entity => ({
                            ...entity,
                            Deleted: true
                        }));
                    }
                }

            })
        ).subscribe()
    }
    ReLoadApiScopes() {
        this.http.get<Array<IMScopeDto>>(`api/idp/api-scopes`, { headers: this.getHeaders() }).pipe(
            tap(users => this.identityApiScopesStore.set(users))
        ).subscribe();
    }

    GetAllApiScopes(force?: boolean) {
        if (force || !this.identityApiScopesQuery.getHasCache()) {
            this.ReLoadApiScopes()
        }

        return this.identityApiScopesQuery.selectAll();
    }

    GetApiScope(id: string) {
        return this.http.get<IMScopeDto>(`api/idp/api-scopes/${id}`, { headers: this.getHeaders() })
    }

    CreateApiScope(dtoModel: IMScopeDto) {
        return this.http.post(`api/idp/api-scopes`, dtoModel, { headers: this.getHeaders() });
    }

    UpdateApiScope(dtoModel: IMScopeDto) {
        return this.http.put(`api/idp/api-scopes`, dtoModel, { headers: this.getHeaders() });
    }

    DeleteApiScope(...ids: string[]) {

        const options = {
            body: ids,
            headers: this.getHeaders()
        }

        return this.http.request("delete", `api/idp/api-scopes`, options);
    }

    //#endregion


}