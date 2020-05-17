import { Component, Input, EventEmitter, Output, ViewContainerRef, ChangeDetectorRef, ComponentFactoryResolver, TemplateRef } from "@angular/core";
import { ListItem } from '../endpoint-rules-list/list-item';
import { EndpointAction } from '../models/endpoint-action';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppUIService } from '@services';
import { DoobModalService, DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { CdkDragDrop, copyArrayItem, moveItemInArray } from '@angular/cdk/drag-drop';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { ActionHelper } from '../endpoint-actions/action-helper';

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
        private componentFactoryResolver: ComponentFactoryResolver) {

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
        this.ContextMenu = this.overlay.OpenContextMenu($event, contextMenu, this.viewContainerRef, {})
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

        return ActionHelper.GetIcon(action);
    }

    public prepareIcon(icon: string) {
    
        if(!icon)
          return null;
    
        if(icon.startsWith('fa#')) {
    
          let res = {
            type: 'fa',
            icon: null
          }
    
          icon = icon.substring(3);
    
          if(icon.includes('|')) {
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