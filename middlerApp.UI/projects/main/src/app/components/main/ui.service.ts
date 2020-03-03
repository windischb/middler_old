import { Injectable } from "@angular/core";
import { BehaviorSubject } from 'rxjs';
import { UIContext } from './ui-context';
import { Router, ChildActivationStart } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class UIService {

    private _uiContext = new UIContext();
    private UIContextSubject$ = new BehaviorSubject<UIContext>(this._uiContext)
    public UIContext$ = this.UIContextSubject$.asObservable();


    get HeaderTitle() {
        return this._uiContext.HeaderTitle;
    }
    set HeaderTitle(value: string) {
        this._uiContext.HeaderTitle = value;
        this.propagateChanges();
    }

    get HeaderSubTitle() {
        return this._uiContext.HeaderSubTitle;
    }
    set HeaderSubTitle(value: string) {
        this._uiContext.HeaderSubTitle = value;
        this.propagateChanges();
    }

    get HeaderIcon() {
        return this._uiContext.HeaderIcon;
    }
    set HeaderIcon(value: string) {
        this._uiContext.HeaderIcon = value;
        this.propagateChanges();
    }

    constructor(private router: Router) {
        this.router.events.subscribe(event => {
            if (event instanceof ChildActivationStart) {
                this._uiContext = new UIContext();
                this.propagateChanges();
            }
        });

    }

    propagateChanges() {
        this.UIContextSubject$.next(this._uiContext);
    }
}
