import { Component, Input, ChangeDetectionStrategy, EventEmitter, Output, ViewChild, TemplateRef, ElementRef, ViewContainerRef, ChangeDetectorRef, ComponentRef, Injector, HostListener, AfterViewInit } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { BehaviorSubject, Observable, Subscription, fromEvent } from 'rxjs';
import { filter, take, map, tap } from 'rxjs/operators';
import { OverlayRef, Overlay } from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { RulesService } from './rules.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MiddlerAction } from './models/middler-action';
import { ActionHelper } from './actions/action-helper';
import { AppUIService } from '../../shared/services/app-ui.service';


declare var $: any;

@Component({
    selector: 'middler-rules',
    templateUrl: './rules-list.component.html',
    styleUrls: ['./rules-list.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: RulesListComponent,
        multi: true
    }],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RulesListComponent implements ControlValueAccessor, AfterViewInit {

    @ViewChild('contextMenu') contextMenu: TemplateRef<any>;
    @ViewChild('addRuleTemplate') addRuleTemplate: TemplateRef<any>;
    overlayRef: OverlayRef | null;
    sub: Subscription;

    private _rules: Array<ListItem<MiddlerRuleDto>> = [];
    private rulesSubject$ = new BehaviorSubject<Array<ListItem<MiddlerRuleDto>>>(this._rules);
    @Input()
    set rules(value: Array<MiddlerRuleDto>) {

        this._rules =  (value || []).map(v => {
            const f = this._rules.find(r => r.Item.Id === v.Id)
            if(f) {
                f.Item = v
                return f;
            } else {
                return new ListItem(v)
            }

        })

        //this._rules = (value || []).map(act => new ListItem(act));
        this.rulesSubject$.next(this._rules)
    }
    get rules() {
        return this._rules.map(la => la.Item);
    }

    rules$: Observable<Array<ListItem<MiddlerRuleDto>>> = this.rulesSubject$.asObservable();
    middlerRules$ = this.rulesService.MiddlerRules$;

    @Output() rulesChanged: EventEmitter<Array<MiddlerRuleDto>> = new EventEmitter<Array<MiddlerRuleDto>>();

    selected: Array<string> = []

    constructor(private ui: AppUIService, public overlay: Overlay, public viewContainerRef: ViewContainerRef, private cref: ChangeDetectorRef, private rulesService: RulesService, private router: Router, private route: ActivatedRoute) {

        ui.Set(value => {
            value.Header.Title = "Endpoint Rules"
            value.Header.Icon = 'stream';
            value.Content.Container = false;
        })


    }


    ngOnInit() {

        this.rulesService.MiddlerRules$.pipe(
            tap(rules => {
                this.rules = rules
            })
        ).subscribe()
    }

    ngAfterViewInit() {
        this.ui.Set(ui => ui.Footer.Outlet = this.addRuleTemplate)
    }


    clickAction($event: MouseEvent, action: ListItem) {
        //console.log($event, action)
        if ($event.ctrlKey) {
            action.Selected = !action.Selected
        } else {

            this._rules = this._rules.map(act => {
                if (act == action) {
                    act.Selected = true;
                } else {
                    act.Selected = false;
                }
                return act;
            })
        }
    }

    contextAction($event: MouseEvent, action: ListItem) {


        const selected = this._rules.filter(sct => sct.Selected);

        if (selected.length == 0 || !selected.includes(action)) {
            this._rules = this._rules.map(act => {
                if (act == action) {
                    act.Selected = true;
                } else {
                    act.Selected = false;
                }
                return act;
            })
        }
        this.open($event)

        // if ($event.ctrlKey) {
        //     action.Selected = !action.Selected
        // } else {
        //     this._rules = this._rules.map(act => {
        //         if (act == action) {
        //             act.Selected = true;
        //         } else {
        //             act.Selected = false;
        //         }
        //         return act;
        //     })
        // }
    }

    DeSelectAll() {
        console.log("Deselect")
        this._rules = this._rules.map(act => {
            act.Selected = false;
            return act;
        })
    }

    SetRuleEnabled(rule: MiddlerRuleDto, value: boolean) {

        this.rulesService.SetRuleEnabled(rule, value)
    }

    GetActionByIndex(index: number) {
        return this.rules[index];
    }

    GetActionById(id: string) {
        return this.rules.find(ac => ac.Id === id);
    }

    ClearSelection() {
        this.selected = [];
        this.cref.markForCheck();
    }


    BuildContext() {
        const cont = new ListContext();
        cont.AllItems = this.rules
        cont.Selected = this._rules.filter(sct => sct.Selected).map(li => li.Item);
        return cont;
    }

    initContextMenu($event: ElementRef) {
        $($event.nativeElement).dropdown({})
    }

    AddRuleOnEnd() {

        let rule = new MiddlerRuleDto();

        rule.Enabled = false;
        rule.Order = this.rulesService.GetNextLastOrder();
        this.rulesService.Add(rule).subscribe();

    }

    open($event: MouseEvent) {

        if ($event.ctrlKey) {
            return;
        }

        $event.preventDefault();


        const clickTarget = event.target as HTMLElement;
        const isList = clickTarget.classList.contains('cdk-drop-list');

        if (isList) {
            this.DeSelectAll();
        }

        this.close();

        const { x, y } = $event;

        const positionStrategy = this.overlay.position()
            .flexibleConnectedTo({ x, y })
            .withPositions([
                {
                    originX: 'start',
                    originY: 'bottom',
                    overlayX: 'start',
                    overlayY: 'top',
                }
            ]);

        this.overlayRef = this.overlay.create({
            positionStrategy,
            scrollStrategy: this.overlay.scrollStrategies.close()
        });

        this.overlayRef.attach(new TemplatePortal(this.contextMenu, this.viewContainerRef, {
            $implicit: this.BuildContext()
        }));

        this.sub = fromEvent<MouseEvent>(document, 'click')
            .pipe(
                filter(event => {
                    const clickTarget = event.target as HTMLElement;
                    return !!this.overlayRef && !this.overlayRef.overlayElement.contains(clickTarget);
                }),
                take(1)
            ).subscribe(() => {
                this.close()

            })

    }

    close() {
        this.sub && this.sub.unsubscribe();
        if (this.overlayRef) {
            this.overlayRef.dispose();
            this.overlayRef = null;
        }
    }

    RemoveRules(rules: Array<MiddlerRuleDto>) {

        rules.forEach(act => this.rulesService.Remove(act.Id).subscribe());
        this.close()
    }

    public SaveOrder() {
        this.rulesService.UpdateRulesOrder();
    }

    editRule(id: string) {
        this.router.navigate([id], { relativeTo: this.route })
    }

    createRule(rule?: MiddlerRuleDto, position?: string) {

        if (rule && position) {

            let index = null;
            const ruleindex = this.rules.findIndex(r => r.Order === rule.Order)

            if (position == 'before') {
                if (ruleindex == 0) {
                    index = 'first'
                } else {
                    index = ruleindex
                }
            } else if (position == 'after') {
                if (ruleindex == this.rules.length - 1) {
                    index = 'last'
                } else {
                    index = ruleindex + 1
                }
            }
            this.router.navigate(["create"], {
                queryParams: { index: index },
                relativeTo: this.route
            })
        } else {
            this.router.navigate(["create"], { relativeTo: this.route })
        }


        this.close();
    }

    calculateRuleOrder(index: number) {


        if (this.rules.length === 0) {
            return 10;
        }

        if (index === 0) {
            return this.rules[0].Order / 2
        }

        if (index >= this.rules.length) {
            return this.rulesService.GetNextLastOrder();
        }

        const before = this.rules[index - 1].Order
        const after = this.rules[index].Order

        return ((after - before) / 2) + before;

    }


    drop(event: CdkDragDrop<string[]>) {


        if (event.previousContainer === event.container) {
            const currentItem = this.rulesService.MiddlerRules[event.previousIndex];

            let direction = null;
            if (event.previousIndex < event.currentIndex) {
                direction = "forward"
            } else if (event.previousIndex > event.currentIndex) {
                direction = "back"
            }

            if (!direction) {
                return;
            }

            if (direction === 'forward') {
                if (event.currentIndex === this.rulesService.MiddlerRules.length - 1) {
                    currentItem.Order = this.rulesService.GetNextLastOrder()
                } else {
                    const beforeItem = this.rulesService.MiddlerRules[event.currentIndex]
                    const nextItem = this.rulesService.MiddlerRules[event.currentIndex + 1]
                    currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
                }
            } else if (direction === 'back') {
                if (event.currentIndex === 0) {
                    currentItem.Order = this.rulesService.MiddlerRules[0].Order / 2
                } else {
                    const beforeItem = this.rulesService.MiddlerRules[event.currentIndex - 1]
                    const nextItem = this.rulesService.MiddlerRules[event.currentIndex]
                    currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
                }
            }


            moveItemInArray(this.rulesService.MiddlerRules, event.previousIndex, event.currentIndex);
            this.rulesService.MiddlerRules = [...this.rulesService.MiddlerRules]
            this.SaveOrder();
        } else {

            //console.log(this.calculateRuleOrder(event.currentIndex))
            let rule = new MiddlerRuleDto();

            rule.Enabled = false;
            console.log(event)
            rule.Order = this.calculateRuleOrder(event.currentIndex)
            this.rulesService.Add(rule).subscribe();
            // this.rulesService.Add(rule);


        }




    }

    GetIcon(action: MiddlerAction) {
        return ActionHelper.GetIcon(action);
    }


    private propagateChange(value: Array<MiddlerRuleDto>) {
        this.rulesChanged.next(value);
        this.registered.forEach(fn => {
            fn(value);
        });

    }

    ///ControlValueAccessor

    writeValue(obj: Array<MiddlerRuleDto>): void {
        this.rules = obj;
    }

    registered = [];
    registerOnChange(fn: any): void {
        if (this.registered.indexOf(fn) === -1) {
            this.registered.push(fn);
        }
    }

    onTouched = () => { };
    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState?(isDisabled: boolean): void {
        throw new Error("Method not implemented.");
    }

}

export class ListContext {

    AllItems = []
    Selected = []
    get Single() {
        return this.Selected?.length === 1 ? this.Selected[0] : null;
    }

    get IsFirst() {
        if (!this.Single) {
            return null;
        }

        return this.AllItems[0].Id === this.Single.Id
    }

    get IsLast() {
        if (!this.Single) {
            return null;
        }

        return this.AllItems[this.AllItems.length - 1].Id === this.Single.Id
    }

}

class ListItem<T = any> {
    Selected: boolean

    private _showDetails: boolean = false;
    set ShowDetails(value: boolean) {
        this._showDetails = value;
        this.ShwoDetailsSubject$.next(this._showDetails);
    }
    get ShowDetails() {
        return this._showDetails;
    }

    ShwoDetailsSubject$ = new BehaviorSubject<boolean>(false);
    ShowDetails$ = this.ShwoDetailsSubject$.asObservable();

    constructor(public Item: T) {

    }
}
