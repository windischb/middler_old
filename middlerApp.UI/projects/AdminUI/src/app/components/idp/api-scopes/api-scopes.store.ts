import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { IMScopeDto } from '../models/m-scope-dto';




export interface ApiScopesState extends EntityState<IMScopeDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-api-scopes', idKey: 'Id' })
export class ApiScopesStore extends EntityStore<ApiScopesState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class ApiScopesQuery extends QueryEntity<ApiScopesState> {
    constructor(protected store: ApiScopesStore) {
        super(store);
    }
}