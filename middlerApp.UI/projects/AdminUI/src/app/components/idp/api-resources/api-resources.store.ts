import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { IMApiResourceListDto } from '../models/m-api-resource-list-dto';



export interface ApiResourcesState extends EntityState<IMApiResourceListDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-api-resources', idKey: 'Id' })
export class ApiResourcesStore extends EntityStore<ApiResourcesState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class ApiResourcesQuery extends QueryEntity<ApiResourcesState> {
    constructor(protected store: ApiResourcesStore) {
        super(store);
    }
}