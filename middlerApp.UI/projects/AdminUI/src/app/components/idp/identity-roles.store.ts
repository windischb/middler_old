import { Store, StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { MRoleDto } from './models/m-role-dto';
import { Injectable } from '@angular/core';
import { MUserDto } from './models/m-user-dto';


export interface IdentityRolesState extends EntityState<MRoleDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-roles', idKey: 'Id' })
export class IdentityRolesStore extends EntityStore<IdentityRolesState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class IdentityRolesQuery extends QueryEntity<IdentityRolesState, MRoleDto> {
    constructor(protected store: IdentityRolesStore) {
        super(store);
    }
}