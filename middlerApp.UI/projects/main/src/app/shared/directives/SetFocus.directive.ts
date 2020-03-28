import { Directive, ElementRef, AfterViewInit, Input } from '@angular/core';

@Directive({
    selector: '[setFocus]'
})
export class SetFocusDirective implements AfterViewInit {

    @Input() setFocus: boolean;

    constructor(private elementRef: ElementRef) {

    }

    ngAfterViewInit(): void {


        const focus = (this.setFocus != null && this.setFocus !== undefined) ? this.setFocus : true;

        if (focus) {
            setTimeout(() => this.elementRef.nativeElement.focus(), 10);
        }


    }


}
