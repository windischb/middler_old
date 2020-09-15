import { StoreConfig, EntityState, EntityStore, QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { IMClientDto } from '../models/m-client-dto';


export interface IdentityClientsState extends EntityState<IMClientDto, string> { }

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'identity-clients', idKey: 'Id' })
export class IdentityClientsStore extends EntityStore<IdentityClientsState> {
    constructor() {
        super();
    }
}

@Injectable({
    providedIn: 'root'
})
export class IdentityClientsQuery extends QueryEntity<IdentityClientsState> {
    constructor(protected store: IdentityClientsStore) {
        super(store);
    }
}