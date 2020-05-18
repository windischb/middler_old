import { Component, Input, ViewChild, TemplateRef, ElementRef, EventEmitter, Output, ChangeDetectorRef, ViewContainerRef, ChangeDetectionStrategy } from "@angular/core";
import { EndpointRule } from '../models/endpoint-rule';
import { EndpointRulesService } from '../endpoint-rules.service';
import { tap } from 'rxjs/operators';
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { ListItem } from './list-item';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { OverlayRef } from '@angular/cdk/overlay';
import { Router, ActivatedRoute } from '@angular/router';
import { EndpointAction } from '../models/endpoint-action';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { ListContext } from './list-context';
import { ActionHelperService } from '../endpoint-actions';

@Component({
    selector: 'enpoint-rules-list',
    templateUrl: './endpoint-rules-list.component.html',
    styleUrls: ['./endpoint-rules-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class EndpointRulesListComponent {

    @ViewChild('contextMenu') contextMenu: TemplateRef<any>;
    @ViewChild('addRuleTemplate') addRuleTemplate: TemplateRef<any>;
    overlayRef: OverlayRef | null;
    sub: Subscription;

    private _rules: Array<ListItem<EndpointRule>> = [];
    private rulesSubject$ = new BehaviorSubject<Array<ListItem<EndpointRule>>>(this._rules);
    @Input()
    set rules(value: Array<EndpointRule>) {

        this._rules = (value || []).map(v => {
            const f = this._rules.find(r => r.Item.Id === v.Id)
            if (f) {
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

    rules$: Observable<Array<ListItem<EndpointRule>>> = this.rulesSubject$.asObservable();
    @Output() rulesChanged: EventEmitter<Array<EndpointRule>> = new EventEmitter<Array<EndpointRule>>();

    selected: Array<string> = []
    constructor(
        private rulesService: EndpointRulesService,
        private cref: ChangeDetectorRef,
        private overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private router: Router,
        private route: ActivatedRoute,
        private actionHelper: ActionHelperService) {

    }

    ngOnInit() {

        this.rulesService.MiddlerRules$.pipe(
            tap(rules => {
                this.rules = rules
            })
        ).subscribe()
    }

    ngAfterViewInit() {
        //this.ui.Set(ui => ui.Footer.Outlet = this.addRuleTemplate)
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

    // contextAction($event: MouseEvent, action: ListItem) {


    //     const selected = this._rules.filter(sct => sct.Selected);

    //     if (selected.length == 0 || !selected.includes(action)) {
    //         this._rules = this._rules.map(act => {
    //             if (act == action) {
    //                 act.Selected = true;
    //             } else {
    //                 act.Selected = false;
    //             }
    //             return act;
    //         })
    //     }
    //     this.open($event)

    //     // if ($event.ctrlKey) {
    //     //     action.Selected = !action.Selected
    //     // } else {
    //     //     this._rules = this._rules.map(act => {
    //     //         if (act == action) {
    //     //             act.Selected = true;
    //     //         } else {
    //     //             act.Selected = false;
    //     //         }
    //     //         return act;
    //     //     })
    //     // }
    // }

    private ContextMenu: IOverlayHandle;

    openOuterContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>) {
        $event.stopPropagation();
        this.ContextMenu?.Close();
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, null)
    }

    openItemContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>, item: ListItem) {
        $event.stopPropagation();


        const selected = this._rules.filter(sct => sct.Selected);

        if (selected.length == 0 || !selected.includes(item)) {
            this._rules = this._rules.map(act => {
                if (act == item) {
                    act.Selected = true;
                } else {
                    act.Selected = false;
                }
                return act;
            })
        }

        this.ContextMenu?.Close();
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, this.BuildContext())
    }

    DeSelectAll() {
        console.log("Deselect")
        this._rules = this._rules.map(act => {
            act.Selected = false;
            return act;
        })
    }

    SetRuleEnabled(rule: EndpointRule, value: boolean) {

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
        const cont = new ListContext<EndpointRule>();
        cont.AllItems = this.rules
        cont.Selected = this._rules.filter(sct => sct.Selected).map(li => li.Item);
        return cont;
    }

    initContextMenu($event: ElementRef) {
        //$($event.nativeElement).dropdown({})
    }

    // AddRuleOnEnd() {

    //     let rule = new EndpointRule();

    //     rule.Enabled = false;
    //     rule.Order = this.rulesService.GetNextLastOrder();
    //     this.rulesService.Add(rule).subscribe();

    //     this.ContextMenu.Close();

    // }

    CreateRuleOnTop() {
        let rule = new EndpointRule();
        rule.Enabled = false;
        if (!this.rules || this.rules.length == 0) {
            rule.Order = 10;
        } else {
            rule.Order = this.rules[0].Order / 2;
        }
        this.rulesService.Add(rule).subscribe();
        this.ContextMenu.Close();
    }

    CreateRuleBefore(item: EndpointRule) {

        const ruleindex = this.rules.findIndex(r => r.Order === item.Order)

        let rule = new EndpointRule();
        rule.Enabled = false;

        if (ruleindex == 0) {
            rule.Order = item.Order / 2;
        } else {
            rule.Order = item.Order - ((item.Order - this.rules[ruleindex - 1].Order) / 2)
        }

        console.log(rule);
        this.rulesService.Add(rule).subscribe();

        this.ContextMenu.Close();
    }

    CreateRuleAfter(item: EndpointRule) {

        const ruleindex = this.rules.findIndex(r => r.Order === item.Order)

        let rule = new EndpointRule();
        rule.Enabled = false;

        if (ruleindex == this.rules.length - 1) {
            rule.Order = this.rulesService.GetNextLastOrder();
        } else {
            rule.Order = item.Order + ((this.rules[ruleindex + 1].Order - item.Order) / 2)
        }

        console.log(rule);
        this.rulesService.Add(rule).subscribe();

        this.ContextMenu.Close();
    }

    CreateRuleOnBottom() {
        let rule = new EndpointRule();
        rule.Enabled = false;
        rule.Order = this.rulesService.GetNextLastOrder();
        this.rulesService.Add(rule).subscribe();
        this.ContextMenu.Close();
    }

    // open($event: MouseEvent) {

    //     // if ($event.ctrlKey) {
    //     //     return;
    //     // }

    //     // $event.preventDefault();


    //     // const clickTarget = event.target as HTMLElement;
    //     // const isList = clickTarget.classList.contains('cdk-drop-list');

    //     // if (isList) {
    //     //     this.DeSelectAll();
    //     // }

    //     // this.close();

    //     // const { x, y } = $event;

    //     // const positionStrategy = this.overlay.position()
    //     //     .flexibleConnectedTo({ x, y })
    //     //     .withPositions([
    //     //         {
    //     //             originX: 'start',
    //     //             originY: 'bottom',
    //     //             overlayX: 'start',
    //     //             overlayY: 'top',
    //     //         }
    //     //     ]);

    //     // this.overlayRef = this.overlay.create({
    //     //     positionStrategy,
    //     //     scrollStrategy: this.overlay.scrollStrategies.close()
    //     // });

    //     // this.overlayRef.attach(new TemplatePortal(this.contextMenu, this.viewContainerRef, {
    //     //     $implicit: this.BuildContext()
    //     // }));

    //     // this.sub = fromEvent<MouseEvent>(document, 'click')
    //     //     .pipe(
    //     //         filter(event => {
    //     //             const clickTarget = event.target as HTMLElement;
    //     //             return !!this.overlayRef && !this.overlayRef.overlayElement.contains(clickTarget);
    //     //         }),
    //     //         take(1)
    //     //     ).subscribe(() => {
    //     //         this.close()

    //     //     })

    // }

    // close() {
    //     this.sub && this.sub.unsubscribe();
    //     if (this.overlayRef) {
    //         this.overlayRef.dispose();
    //         this.overlayRef = null;
    //     }
    // }

    RemoveRules(rules: Array<EndpointRule>) {
        rules.forEach(act => this.rulesService.Remove(act.Id).subscribe());
        this.ContextMenu.Close()
    }

    public SaveOrder() {
        this.rulesService.UpdateRulesOrder();
    }

    editRule(id: string) {
        this.router.navigate([id], { relativeTo: this.route })
    }

    // createRule(rule?: EndpointRule, position?: string) {

    //     if (rule && position) {

    //         let index = null;
    //         const ruleindex = this.rules.findIndex(r => r.Order === rule.Order)

    //         if (position == 'before') {
    //             if (ruleindex == 0) {
    //                 index = 'first'
    //             } else {
    //                 index = ruleindex
    //             }
    //         } else if (position == 'after') {
    //             if (ruleindex == this.rules.length - 1) {
    //                 index = 'last'
    //             } else {
    //                 index = ruleindex + 1
    //             }
    //         }
    //         this.router.navigate(["create"], {
    //             queryParams: { index: index },
    //             relativeTo: this.route
    //         })
    //     } else {
    //         this.router.navigate(["create"], { relativeTo: this.route })
    //     }


    //     this.close();
    // }

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
            let rule = new EndpointRule();

            rule.Enabled = false;
            console.log(event)
            rule.Order = this.calculateRuleOrder(event.currentIndex)
            this.rulesService.Add(rule).subscribe();
            // this.rulesService.Add(rule);


        }




    }


    GetIcon(action: EndpointAction) {

        return this.actionHelper.GetIcon(action);
    }

    public prepareIcon(icon: string) {

        if (!icon)
            return null;

        if (icon.startsWith('fa#')) {

            let res = {
                type: 'fa',
                icon: null
            }

            icon = icon.substring(3);

            if (icon.includes('|')) {
                res.icon = icon.split('|');
            } else {
                res.icon = icon;
            }
            return res;
        }

        return {
            type: 'ant',
            icon: icon
        };
    }


    private propagateChange(value: Array<EndpointRule>) {
        this.rulesChanged.next(value);
        this.registered.forEach(fn => {
            fn(value);
        });

    }

    ///ControlValueAccessor

    writeValue(obj: Array<EndpointRule>): void {
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
