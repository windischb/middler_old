import { Component, ContentChildren, QueryList, Input, EventEmitter, Output, ElementRef, ChangeDetectionStrategy } from "@angular/core";
import { SpectrumTabComponent } from './tab.component';
import { BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Component({
    selector: 'spectrum-tabs',
    templateUrl: './tabs.component.html',
    styles: [
        `
        :host {
            display: flex!important;
            flex-grow: 1;
        }

        .show {
            display: flex!important
        }

        `
    ],
    changeDetection: ChangeDetectionStrategy.OnPush

})
export class SpectrumTabsComponent {

    @Input() mode: 'hide' | 'remove' = 'hide';

    @ContentChildren(SpectrumTabComponent) tabs: QueryList<SpectrumTabComponent>;

    tabsSubject$: BehaviorSubject<Array<SpectrumTabComponent>> = new BehaviorSubject<Array<SpectrumTabComponent>>([]);
    tabs$ = this.tabsSubject$.asObservable();
    tabsContents$ = this.tabs$.pipe(
        map(tabs => {
            return tabs.filter(tab => ((tab.mode || this.mode) === 'hide') || tab.active);
        })
    );

    @Output() activeTab = new EventEmitter<any>();
    @Output() activeTabIndex = new EventEmitter<number>();


    public IndicatorWidth: number = 0;
    public IndicatorLeft: number = 0;

    constructor() {

    }

    ngAfterContentInit() {

        this.tabs.changes.subscribe(tabs => {
            this.propagateTabs(tabs);
        });

        setTimeout(() => {
            this.propagateTabs(this.tabs.toArray());
        }, 0);

    }

    ngAfterViewInit() {
        setTimeout(() => {
            this.propagateTabs(this.tabs.toArray());
        }, 0);
    }


    propagateTabs(tabs: Array<SpectrumTabComponent>) {


        //let active = false;
        //let index = 0;
        let activeTab =  tabs.findIndex(t => t.active);

        /* tabs.forEach(t => {

            if (t.active) {
                if (active) {
                    t.active = false;
                } else {
                    active = true;
                    activeTab = t;
                    //this.activeTab.emit(t.title);
                    //this.activeTabIndex.emit(index);
                }
            }
            index++;
        }); */

        if (activeTab == -1) {
            activeTab = 0;
            //tabs[0].active = true;
        }
        this.Activate(activeTab);
        //this.tabsSubject$.next(tabs);
    }


    public Activate(value: string | number) {

        setTimeout(() => {

            const byIndex = typeof value === 'number';
            const tabs = this.tabs.map((item, _index) => {
                const match = byIndex ? (value === _index) : (value === item.title);
                if (match) {
                    this.activeTab.emit(item.title);
                    this.activeTabIndex.emit(_index);
                    item.active = true;
                    const tabEl = document.getElementById(item.Id);
                    if(tabEl) {
                        this.IndicatorWidth = tabEl.clientWidth + 16
                        this.IndicatorLeft = tabEl.offsetLeft -8
                    }

                    return item;
                } else {
                    item.active = false;
                    return item;
                }
            });

            this.tabsSubject$.next(tabs);

        }, 0);


    }


}
