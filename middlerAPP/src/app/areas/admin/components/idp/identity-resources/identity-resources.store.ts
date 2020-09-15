import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { IMIdentityResourceListDto } from '../models/m-identity-resource-list-dto';



export interface IdentityResourcesState extends EntityState<IMIdentityResourceListDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-resources', idKey: 'Id' })
export class IdentityResourcesStore extends EntityStore<IdentityResourcesState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class IdentityResourcesQuery extends QueryEntity<IdentityResourcesState> {
    constructor(protected store: IdentityResourcesStore) {
        super(store);
    }
}