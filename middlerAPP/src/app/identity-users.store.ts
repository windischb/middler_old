import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { MUserListDto } from './components/identity/models/m-user-list-dto';


export interface IdentityUsersState extends EntityState<MUserListDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-users', idKey: 'Id' })
export class IdentityUsersStore extends EntityStore<IdentityUsersState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class IdentityUsersQuery extends QueryEntity<IdentityUsersState> {
    constructor(protected store: IdentityUsersStore) {
        super(store);
    }
}