import { Component, Input, TemplateRef, ViewChild, ElementRef, Output, EventEmitter, AfterViewInit } from "@angular/core";
import { BehaviorSubject } from 'rxjs';
import * as uuidv4 from "uuid/v4";

@Component({
    selector: 'spectrum-tab',
    templateUrl: './tab.component.html'
})
export class SpectrumTabComponent implements AfterViewInit {

    @Input()content: TemplateRef<any>;
    @Input()title: string;
    @Input()icon: string;
    @Input()mode: 'hide' | 'remove' | null = null;

    @ViewChild('innerContent') innerContent: TemplateRef<any>;


    _active = false;
    active$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    @Input()
    set active(value: boolean) {
        this._active = !!value;
        this.active$.next(!!value);
    }

    get active(): boolean {
        return this._active;
    }

    constructor(private element: ElementRef) {

        this.Id = uuidv4()
    }

    ngAfterViewInit() {
        //console.log(this.innerContent)
    }

    public Id: string;


}
