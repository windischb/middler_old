import { Injectable } from '@angular/core';
import { StoreConfig, Query, Store } from '@datorama/akita';


export interface AppUIState {
    sideBarCollapsed: boolean;
    showDebugInformations: boolean;
}

export function createInitialState(): AppUIState {
    return {
        sideBarCollapsed: false,
        showDebugInformations: false
    };
}


@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'app-ui' })
export class AppUIStore extends Store<AppUIState> {
    constructor() {
        super(createInitialState());
    }
}

@Injectable({
    providedIn: 'root'
})
export class AppUIQuery extends Query<AppUIState> {

    sideBarCollapsed$ = this.select(state => state.sideBarCollapsed);
    showDebugInformations$ = this.select(state => state.showDebugInformations);

    constructor(protected store: AppUIStore) {
        super(store);
    }
}