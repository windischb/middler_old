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

    Set(value: ((context: UIContext) => void)) {

        value(this._uiContext);
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
