import { Component, Input, EventEmitter, Output, ViewContainerRef, ChangeDetectorRef, ComponentFactoryResolver, TemplateRef } from "@angular/core";
import { ListItem } from '../endpoint-rules-list/list-item';
import { EndpointAction } from '../models/endpoint-action';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppUIService } from '@services';
import { DoobModalService, DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { CdkDragDrop, copyArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { ActionHelperService } from '../endpoint-actions/action-helper';
import { UrlRedirectAction, UrlRewriteAction, ScriptAction } from '../endpoint-actions';
import { ListContext } from '../endpoint-rules-list/list-context';

@Component({
    selector: 'endpoint-actions-list',
    templateUrl: './endpoint-actions-list.component.html',
    styleUrls: ['./endpoint-actions-list.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: EndpointActionsListComponent,
        multi: true
    }],
})
export class EndpointActionsListComponent implements ControlValueAccessor {


    private _actions: Array<ListItem<EndpointAction>> = [];
    private actionsSubject$ = new BehaviorSubject<Array<ListItem<EndpointAction>>>(this._actions);
    @Input()
    set actions(value: Array<EndpointAction>) {
        this._actions = (value || []).map(act => new ListItem(act));
        this.actionsSubject$.next(this._actions)
    }
    get actions() {
        return this._actions.map(la => la.Item);
    }

    actions$: Observable<Array<ListItem<EndpointAction>>> = this.actionsSubject$.asObservable();

    @Output() actionsChanged: EventEmitter<Array<EndpointAction>> = new EventEmitter<Array<EndpointAction>>();

    constructor(
        private uiService: AppUIService,
        private modal: DoobModalService,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private cref: ChangeDetectorRef,
        private componentFactoryResolver: ComponentFactoryResolver,
        private actionHelper: ActionHelperService) {

    }

    private ContextMenu: IOverlayHandle;
    openOuterContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>) {
        $event.stopPropagation();
        this.ContextMenu?.Close();
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, null)
    }

    openItemContextMenu($event: MouseEvent, contextMenu: TemplateRef<any>, item: ListItem) {
        $event.stopPropagation();


        const selected = this._actions.filter(sct => sct.Selected);

        if (selected.length == 0 || !selected.includes(item)) {
            this._actions = this._actions.map(act => {
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

    BuildContext() {
        const cont = new ListContext<EndpointAction>();
        cont.AllItems = this.actions
        cont.Selected = this._actions.filter(sct => sct.Selected).map(li => li.Item);
        return cont;
    }

    clickAction($event: MouseEvent, action: ListItem) {

        if ($event.ctrlKey) {
            action.Selected = !action.Selected
        } else {
            this._actions = this._actions.map(act => {
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

        const selected = this._actions.filter(sct => sct.Selected);

        if (selected.length == 0 || !selected.includes(action)) {
            this._actions = this._actions.map(act => {
                if (act == action) {
                    act.Selected = true;
                } else {
                    act.Selected = false;
                }
                return act;
            })
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

        if (position == 'top') {
            this.actions = [act, ...this.actions]
            this.propagateChange(this.actions)
            return;
        }

        if (position == 'bottom') {
            this.actions = [...this.actions, act]
            this.propagateChange(this.actions)
            return;
        }

        const actionindex = this.actions.findIndex(r => r === currentAction)
        let acts = [...this.actions];

        if (position == 'before') {
            if (actionindex == 0) {
                this.actions = [act, ...this.actions]
            } else {
                acts.splice(actionindex, 0, act)
                this.actions = [...acts]
            }
            
        }

        if (position == 'after') {
            if (actionindex == this.actions.length -1) {
                this.actions = [...this.actions, act]
            } else {

                acts.splice(actionindex+1, 0, act)
                this.actions = [...acts]
            }
            
        }

        this.propagateChange(this.actions)
    }

    SetActionEnabled(action: EndpointAction, value: boolean) {
        this.actions = this.actions.map(act => {
            if(act == action) {
                act.Enabled = value;
            }
            return act;
        });
        this.propagateChange(this.actions)
    }

    RemoveActions(actions: Array<EndpointAction>) {

        actions.forEach(act => this.actions = this.actions.filter(ac => ac != act));
        this.propagateChange(this.actions)
        this.ContextMenu?.Close();
    }

    drop(event: CdkDragDrop<EndpointAction[]>) {

        if (event.previousContainer !== event.container) {
            let act = [...this.actions]
            let action = JSON.parse(JSON.stringify(event.previousContainer.data))
            copyArrayItem(action, act, event.previousIndex, event.currentIndex);
            this.actions = [...act]
            this.propagateChange(this.actions)
        } else {
            let act = [...this.actions]
            moveItemInArray(act, event.previousIndex, event.currentIndex);
            this.actions = [...act]
            this.propagateChange(this.actions)
        }

    }

    openModal(action: EndpointAction) {

        this.ContextMenu?.Close();
        switch (action.ActionType) {
            case 'UrlRedirect': {
                this.actionHelper.GetModal('UrlRedirect').SetData(action)
                    .AddEventHandler("OK", context => {
                        action.Parameters = context.payload;
                        this.propagateChange([...this.actions]);
                    }).Open()

                break;
            }
            case 'UrlRewrite': {
                this.actionHelper.GetModal('UrlRewrite')
                    .SetData(action)
                    .AddEventHandler("OK", context => {
                        action.Parameters = context.payload;
                        this.propagateChange([...this.actions]);
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
                        this.propagateChange([...this.actions]);
                    }).Open()
                break;
            }
        }

    }

    private propagateChange(value: Array<EndpointAction>) {

        this.actionsChanged.next(value);
        this.registered.forEach(fn => {
            fn(value);
        });
        this.cref.detectChanges();
    }

    ///ControlValueAccessor

    writeValue(obj: Array<EndpointAction>): void {
        this.actions = obj;
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