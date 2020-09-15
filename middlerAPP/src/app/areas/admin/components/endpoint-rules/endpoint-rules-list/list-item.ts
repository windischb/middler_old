import { BehaviorSubject } from "rxjs";

export class ListItem<T = any> {
    Selected: boolean

    private _showDetails: boolean = false;
    set ShowDetails(value: boolean) {
        this._showDetails = value;
        this.ShowDetailsSubject$.next(this._showDetails);
    }
    get ShowDetails() {
        return this._showDetails;
    }

    ShowDetailsSubject$ = new BehaviorSubject<boolean>(false);
    ShowDetails$ = this.ShowDetailsSubject$.asObservable();

    constructor(public Item: T) {

    }
}