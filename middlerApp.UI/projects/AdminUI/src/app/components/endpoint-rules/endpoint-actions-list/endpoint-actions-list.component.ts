import { Component, Input, EventEmitter, Output, ViewContainerRef, ChangeDetectorRef, ComponentFactoryResolver, TemplateRef, ChangeDetectionStrategy, OnInit } from "@angular/core";
import { ListItem } from '../endpoint-rules-list/list-item';
import { EndpointAction } from '../models/endpoint-action';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppUIService } from '@services';
import { DoobModalService, DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { CdkDragDrop, copyArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';
import { ActionHelperService } from '../endpoint-actions/action-helper';
import { UrlRedirectAction, UrlRewriteAction, ScriptAction } from '../endpoint-actions';
import { ListContext } from '../endpoint-rules-list/list-context';
import { EndpointRulesService } from '../endpoint-rules.service';
import { compare } from 'fast-json-patch';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'endpoint-actions-list',
    templateUrl: './endpoint-actions-list.component.html',
    styleUrls: ['./endpoint-actions-list.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: EndpointActionsListComponent,
        multi: true
    }],
})
export class EndpointActionsListComponent implements OnInit, ControlValueAccessor {


    // @Input() ruleId: string;

    private actionsSubject$ = new BehaviorSubject<Array<EndpointAction>>(null);
    actions$ = this.actionsSubject$.asObservable();

    @Input()
    get actions() {
        return this.actionsSubject$.value;
    }
    set actions(value: Array<EndpointAction>) {

        value = value?.sort((a, b) => {
            if (a.Order > b.Order) {
                return 1;
            } else if (a.Order < b.Order) {
                return -1;
            }
            return 0;
        })

        this.actionsSubject$.next(value);
    }

    @Output() actionsChange: EventEmitter<Array<EndpointAction>> = new EventEmitter<Array<EndpointAction>>();

    selectedActions: Array<string> = [];

    constructor(
        private uiService: AppUIService,
        private modal: DoobModalService,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private cref: ChangeDetectorRef,
        private componentFactoryResolver: ComponentFactoryResolver,
        private actionHelper: ActionHelperService,
        private rulesService: EndpointRulesService,) {

    }

    ngOnInit() {

        // this.rulesService.GetActionsForRule(this.ruleId).subscribe(actions => {
        //     this.actions = actions;
        // })
    }

    IsActionSelected(action: EndpointAction) {
        return this.selectedActions.includes(action.Id);
    }

    private setActionSelected(action: EndpointAction, value: boolean) {

        if (!value) {
            this.selectedActions = this.selectedActions.filter(actId => actId != action.Id);
        } else {
            this.selectedActions = [...new Set([...this.selectedActions, action.Id])]
        }

    }


    GetSelectedActions() {
        return this.actions.filter(act => this.selectedActions.includes(act.Id));
    }



    ClickAction($event: MouseEvent, action: EndpointAction) {
        if ($event.ctrlKey) {
            const isSelected = this.IsActionSelected(action);
            this.setActionSelected(action, !isSelected)
        } else {
            this.selectedActions = [];
            this.setActionSelected(action, true);
        }
    }






    private ContextMenu: IOverlayHandle;
    OpenOuterContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>) {
        $event.stopPropagation();
        this.ContextMenu?.Close();
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, null)
    }

    openItemContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>, action: EndpointAction) {
        $event.stopPropagation();

        if (this.selectedActions.length == 0 || !this.IsActionSelected(action)) {
            this.setActionSelected(action, true);
        }

        this.ContextMenu?.Close();
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, this.BuildContext())
    }

    BuildContext() {
        const cont = new ListContext<EndpointAction>();
        cont.AllItems = this.actions;
        cont.Selected = this.actions.filter(act => this.selectedActions.includes(act.Id));
        return cont;
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

    private newActionsId = 0;
    CreateAction(actionType: string, position: string, currentAction?: EndpointAction) {

        let act = new EndpointAction;


        switch (actionType.toLowerCase()) {
            case 'urlredirect': {
                act = new UrlRedirectAction();
                break;
            }
            case 'urlrewrite': {
                act = new UrlRewriteAction();
                break;
            }
            case 'script': {
                act = new ScriptAction();
                break;
            }
        }

        act.Order = this.CalculateOrder(position, currentAction);
        act.Id = this.getNewActionsId().toString();
        this.actions = [...this.actions, act];
        this.propagateChange(this.actions);
        //this.rulesService.AddAction(this.ruleId, act).subscribe();

    }

    private getNewActionsId() {

        const filterInt = function (value) {
            if (/^(\-|\+)?([0-9]+|Infinity)$/.test(value))
                return Number(value);
            return NaN;
        }

        var numbers = this.actions.map(a => filterInt(a.Id)).filter(n => !Number.isNaN(n)).sort((a, b) => b - a)

        if (numbers && numbers.length > 0) {
            return numbers[0] + 1;
        }

        return 0;
    }

    private GetNextLastOrder() {
        if (!this.actions || this.actions.length === 0) {
            return 10;
        }
        var next = Math.trunc(Math.max(...this.actions.map(r => r.Order)) + 10);
        console.log("NEXT", next)
        return next;
    }
    CalculateOrder(position: string, action?: EndpointAction) {

        let currentIndex = -1;

        if (action) {
            currentIndex = this.actions?.findIndex(r => r.Order === action.Order);
        }


        switch (position) {
            case 'top': {
                if (!this.actions || this.actions.length == 0) {
                    return 10;
                } else {
                    return this.actions[0].Order / 2;
                }
            }
            case 'before': {
                if (currentIndex == 0) {
                    return action.Order / 2;
                } else {
                    return action.Order - ((action.Order - this.actions[currentIndex - 1].Order) / 2)
                }
            }
            case 'after': {
                if (currentIndex == this.actions.length - 1) {
                    return this.GetNextLastOrder();
                } else {
                    return action.Order + ((this.actions[currentIndex + 1].Order - action.Order) / 2)
                }
            }
            case 'bottom': {
                return this.GetNextLastOrder();
            }
        }
    }


    SetActionEnabled(action: EndpointAction, value: boolean) {

        this.actions = this.actions.map(act => {
            if (act == action) {
                act.Enabled = value;
            }
            return act;
        });
        this.propagateChange(this.actions);
        // this.rulesService.SetActionEnabled(this.ruleId, action, value).subscribe(_ => {
        //     this.actions = this.actions.map(act => {
        //         if (act == action) {
        //             act.Enabled = value;
        //         }
        //         return act;
        //     });
        // });

    }

    RemoveActions(actions: Array<EndpointAction>) {

        const ids = actions.map(a => a.Id);
        this.actions = this.actions.filter(act => !ids.includes(act.Id))

        // this.rulesService.RemoveActions(this.ruleId, ...ids).subscribe(_ => {
        //     this.actions = this.actions.filter(act => !ids.includes(act.Id))
        // })
        this.propagateChange(this.actions);
        this.ContextMenu?.Close();

    }

    drop(event: CdkDragDrop<EndpointAction[]>) {

        console.log(event);
        const currentItem = this.actions[event.previousIndex];

        let direction = null;
        if (event.previousIndex < event.currentIndex) {
            direction = "forward"
        } else if (event.previousIndex > event.currentIndex) {
            direction = "back"
        }

        if (!direction) {
            return;
        }

        console.log(direction, this.actions.length)
        if (direction === 'forward') {
            if (event.currentIndex === this.actions.length - 1) {
                console.log("-- 1 --")
                currentItem.Order = this.GetNextLastOrder()
            } else {
                console.log("-- 2 --")
                const beforeItem = this.actions[event.currentIndex]
                const nextItem = this.actions[event.currentIndex + 1]
                currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
            }
        } else if (direction === 'back') {
            if (event.currentIndex === 0) {
                console.log("-- 3 --")
                currentItem.Order = this.actions[0].Order / 2
            } else {
                console.log("-- 4 --")
                const beforeItem = this.actions[event.currentIndex - 1]
                const nextItem = this.actions[event.currentIndex]
                currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
            }
        }


        moveItemInArray(this.actions, event.previousIndex, event.currentIndex);
        this.actions = [...this.actions]
        this.propagateChange(this.actions)
        // this.rulesService.UpdateActionsOrder(this.ruleId, this.actions).subscribe();

    }


    // private generatePatchDocument(base: any, comp: any) {
    //     var patchDocument = compare(base, comp);
    //     return patchDocument;
    // }

    openModal(action: EndpointAction) {

        this.ContextMenu?.Close();
        const origAction = {
            ...action
        }

        switch (action.ActionType) {
            case 'UrlRedirect': {
                this.actionHelper.GetModal('UrlRedirect').SetData(action)
                    .AddEventHandler("OK", context => {
                        action.Parameters = context.payload;
                        this.actions = [...this.actions];
                        this.propagateChange(this.actions);
                        // const patchDoc = this.generatePatchDocument(origAction, action);
                        // this.rulesService.PatchAction(this.ruleId, origAction.Id, patchDoc).subscribe(_ => {
                        //     this.actions = [...this.actions];
                        // })
                    }).Open()

                break;
            }
            case 'UrlRewrite': {
                this.actionHelper.GetModal('UrlRewrite')
                    .SetData(action)
                    .AddEventHandler("OK", context => {
                        action.Parameters = context.payload;
                        this.actions = [...this.actions];
                        this.propagateChange(this.actions);

                        // const patchDoc = this.generatePatchDocument(origAction, action);
                        // this.rulesService.PatchAction(this.ruleId, origAction.Id, patchDoc).subscribe(_ => {
                        //     this.actions = [...this.actions];
                        // })
                    }).Open()
                break;
            }
            // case 'Proxy': {
            //     this.modal.FromComponent(ProxyModalComponent)
            //         .SetModalOptions({
            //             componentFactoryResolver: this.componentFactoryResolver,
            //             overlayConfig: {
            //                 width: "50%",
            //                 maxWidth: "500px"
            //             }
            //         })
            //         .CloseOnOutsideClick()
            //         .SetData(action)
            //         .AddEventHandler("OK", context => {
            //             action.Parameters = context.payload;
            //             this.propagateChange([...this.actions]);
            //         }).Open()
            //     break;
            // }
            case 'Script': {
                this.actionHelper.GetModal('Script')
                    .SetData(action)
                    .AddEventHandler("OK", context => {
                        action.Parameters = context.payload;
                        this.actions = [...this.actions];
                        this.propagateChange(this.actions);
                        // const patchDoc = this.generatePatchDocument(origAction, action);
                        // this.rulesService.PatchAction(this.ruleId, origAction.Id, patchDoc).subscribe(_ => {
                        //     this.actions = [...this.actions];
                        // })
                    }).Open()
                break;
            }
        }

    }


    propagateChange(value: Array<EndpointAction>) {

        this.actionsChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<EndpointAction>): void {
        this.actions = value;
    }

    private registered = [];
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

    }

}