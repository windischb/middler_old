import { Store, StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { MRoleDto } from './components/idp/models/m-role-dto';
import { AppUIHeader } from './shared/models/app-ui-context';
import { Injectable } from '@angular/core';
import { MUserDto } from './components/idp/models/m-user-dto';


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