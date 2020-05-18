import { Injectable } from "@angular/core";
import { BehaviorSubject } from 'rxjs';
import { Router, ChildActivationStart } from '@angular/router';
import { AppUIContext } from '../models/app-ui-context';

@Injectable({
    providedIn: 'root'
})
export class AppUIService {

    private _defaultUiContext = new AppUIContext();

    private _uiContext;
    private UIContextSubject$ = new BehaviorSubject<AppUIContext>(this.BuildNewContext())
    public UIContext$ = this.UIContextSubject$.asObservable();



    Set(value: ((context: AppUIContext) => void)) {
        value(this._uiContext);
        this.propagateChanges();
    }

    SetDefault(value: ((context: AppUIContext) => void)) {
        value(this._defaultUiContext);

    }


    constructor(private router: Router) {
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
}
