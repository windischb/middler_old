import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { IMApiResourceListDto } from '../models/m-api-resource-list-dto';



export interface IdentityApiResourcesState extends EntityState<IMApiResourceListDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-api-resources', idKey: 'Id' })
export class IdentityApiResourcesStore extends EntityStore<IdentityApiResourcesState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class IdentityApiResourcesQuery extends QueryEntity<IdentityApiResourcesState> {
    constructor(protected store: IdentityApiResourcesStore) {
        super(store);
    }
}