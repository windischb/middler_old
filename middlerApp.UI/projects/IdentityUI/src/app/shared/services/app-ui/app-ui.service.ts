import { Injectable } from "@angular/core";
import { BehaviorSubject } from 'rxjs';
import { Router, ChildActivationStart } from '@angular/router';
import { AppUIContext } from '../../models/app-ui-context';
import { AppUIStore, AppUIQuery } from './app-ui.store';

@Injectable({
    providedIn: 'root'
})
export class AppUIService {

    private _defaultUiContext = new AppUIContext();

    private _uiContext;
    private UIContextSubject$ = new BehaviorSubject<AppUIContext>(this.BuildNewContext())
    public UIContext$ = this.UIContextSubject$.asObservable();

    sideBarCollapsed$ = this.appUIQuery.sideBarCollapsed$;
    showDebugInformations$ = this.appUIQuery.showDebugInformations$;

    Set(value: ((context: AppUIContext) => void)) {
        value(this._uiContext);
        this.propagateChanges();
    }

    SetDefault(value: ((context: AppUIContext) => void)) {
        value(this._defaultUiContext);

    }

    

    constructor(
        private router: Router,
        private appUIStore: AppUIStore,
        private appUIQuery: AppUIQuery
    ) {


        this._uiContext = this.BuildNewContext();
        this.router.events.subscribe(event => {
            if (event instanceof ChildActivationStart) {
                this._uiContext = this.BuildNewContext();
                this.propagateChanges();
            }
        });

    }

    private BuildNewContext() {
        return JSON.parse(JSON.stringify(this._defaultUiContext));
    }
    propagateChanges() {
        this.UIContextSubject$.next(this._uiContext);
    }

    toggleSideBar() {
        const isCollapsed = this.appUIStore.getValue().sideBarCollapsed;

        this.appUIStore.update(state => state = {
            ...state,
            sideBarCollapsed: !isCollapsed
        })

    }
}
