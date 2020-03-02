import { Component, Input, ChangeDetectionStrategy, EventEmitter, Output, ViewChild, TemplateRef, ElementRef, ViewContainerRef, ChangeDetectorRef, ComponentRef, Injector } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormArray, FormGroup } from '@angular/forms';
import { MiddlerAction } from './models/middler-action';
import { BehaviorSubject, Observable, Subscription, fromEvent } from 'rxjs';
import { map, filter, take } from 'rxjs/operators';
import { OverlayRef, Overlay, OverlayConfig } from '@angular/cdk/overlay';
import { TemplatePortal, ComponentPortal, PortalInjector } from '@angular/cdk/portal';
import { UrlRedirectParameters, UrlRedirectAction } from './actions/url-redirect-action';
import { ActionEditModalService } from './modal/action-modal.service';
import { ActionEditModalOverlayRef } from './modal/action-edit-modal-overlay-ref';
import { ActionEditModalComponent } from './modal/action-edit-modal.component';
import { ACTION_DIALOG_DATA } from './modal/action-edit-modal.tokens';
import { UrlRedirectModalComponent } from './actions/url-redirect-modal.component';
import { CdkDragDrop, moveItemInArray, transferArrayItem, copyArrayItem } from '@angular/cdk/drag-drop';
import { UrlRewriteAction } from './actions/url-rewrite-action';
import { ActionHelper } from './actions/action-helper';

declare var $: any;

@Component({
    selector: 'middler-rule-actions',
    templateUrl: './actions-list.component.html',
    styleUrls: ['./actions-list.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: ActionsListComponent,
        multi: true
    }],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ActionsListComponent implements ControlValueAccessor {

    @ViewChild('userMenu') userMenu: TemplateRef<any>;
    overlayRef: OverlayRef | null;
    sub: Subscription;

    private _actions: Array<ListItem<MiddlerAction>> = [];
    private actionsSubject$ = new BehaviorSubject<Array<ListItem<MiddlerAction>>>(this._actions);
    @Input()
    set actions(value: Array<MiddlerAction>) {
        this._actions = (value || []).map(act => new ListItem(act));
        this.actionsSubject$.next(this._actions)
    }
    get actions() {
        return this._actions.map(la => la.Item);
    }

    actions$: Observable<Array<ListItem<MiddlerAction>>> = this.actionsSubject$.asObservable();

    @Output() actionsChanged: EventEmitter<Array<MiddlerAction>> = new EventEmitter<Array<MiddlerAction>>();


    newActions: Array<MiddlerAction> = [
        new UrlRedirectAction(),
        new UrlRewriteAction()
    ]


    selected: Array<string> = []

    constructor(private modal: ActionEditModalService, public overlay: Overlay, public viewContainerRef: ViewContainerRef, private cref: ChangeDetectorRef) {

        console.log("Constructor")
        // this.formGroup = this.fb.group({
        //     Actions: this.fb.array([])
        // })



    }


    ngOnInit() {

        // this.actionsSubject$.pipe(
        //     map(arr => {
        //         return arr.map(act => this.buildActionFormgroup(act))
        //     })
        // ).subscribe(arr => {
        //     arr.forEach(element => {
        //         this.ActionsArr.push(element)
        //         console.log(element)
        //     });
        // })
    }

    // private buildActionFormgroup(action?: any) {
    //     const fg = this.fb.group({
    //         ActionType: [null],
    //         ContinueAfterwards: [false],
    //         WriteStreamDirect: [false],
    //         Parameters: this.fb.group({})
    //     })
    //     fg.get('ActionType').valueChanges.subscribe(val => {
    //         switch (val) {
    //             case 'UrlRedirect': {
    //                 fg.setControl('Parameters', this.buildUrlRedirectFormGroup())
    //             }
    //         }
    //     })
    //     if (action) {
    //         fg.patchValue(action)
    //     }

    //     return fg;
    // }

    // private buildUrlRedirectFormGroup() {
    //     const fg = this.fb.group({
    //         RedirectTo: [null],
    //         Permanent: [false],
    //         PreserveMethod: [false]
    //     })
    //     return fg;
    // }


    clickAction($event: MouseEvent, action: ListItem) {

        //const id = this.GetActionByIndex(index)?.Id;
        //console.log(id)
        if ($event.ctrlKey) {
            action.Selected = !action.Selected
            // if(!this.selected.includes(id)) {
            //     this.selected = [...this.selected, id]
            // } else {
            //     this.selected = this.selected.filter(it => it != id)
            // }
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


        //const id = this.GetActionByIndex(index)?.Id;

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

        // //const id = this.GetActionByIndex(index)?.Id;
        // //console.log(id)
        // if ($event.ctrlKey) {
        //     action.Selected = !action.Selected
        //     // if(!this.selected.includes(id)) {
        //     //     this.selected = [...this.selected, id]
        //     // } else {
        //     //     this.selected = this.selected.filter(it => it != id)
        //     // }
        // } else {
        //     this._actions = this._actions.map(act => {
        //         if (act == action) {
        //             act.Selected = true;
        //         } else {
        //             act.Selected = false;
        //         }
        //         return act;
        //     })
        // }
    }



    GetActionByIndex(index: number) {
        return this.actions[index];
    }

    GetActionById(id: string) {
        return this.actions.find(ac => ac.Id === id);
    }

    ClearSelection() {
        this.selected = [];
        this.cref.markForCheck();
    }

    AddAction() {

        var act = new MiddlerAction<UrlRedirectParameters>()
        act.ActionType = "UrlRedirect"
        this.actions = [...this.actions, act]
        this.propagateChange(this.actions)
    }


    BuildContext() {
        const cont = new ListContext();
        cont.AllItems = this.actions
        cont.Selected = this._actions.filter(sct => sct.Selected).map(li => li.Item);
        return cont;
    }

    initContextMenu($event: ElementRef) {
        $($event.nativeElement).dropdown({})
    }

    sideBar: any;

    initSidebar($event: ElementRef) {

        if (!this.sideBar) {
            this.sideBar = $($event.nativeElement);
        }

        this.sideBar.sidebar({
            context: $('.bottom.segment'),
            dimPage: false
        }).sidebar('setting', 'transition', 'overlay')
    }

    openAddSidebar() {
        this.sideBar.sidebar('show')
        this.close();
    }

    open($event: MouseEvent) {

        if ($event.ctrlKey) {
            return;
        }


        $event.preventDefault();
        this.close();


        //const id = this.GetActionByIndex(index)?.Id;

        // const selected = this._actions.filter(sct => sct.Selected);

        // if (selected.length == 0 || !selected.includes(action)) {
        //     this._actions = this._actions.map(act => {
        //         if (act == action) {
        //             act.Selected = true;
        //         } else {
        //             act.Selected = false;
        //         }
        //         return act;
        //     })
        // }
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

        this.overlayRef.attach(new TemplatePortal(this.userMenu, this.viewContainerRef, {
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

    openModal(action: MiddlerAction) {

        switch (action.ActionType) {
            case 'UrlRedirect': {
                this.modal.OpenComponent(UrlRedirectModalComponent, {
                    actionContext: action,
                    onOk: (value: UrlRedirectParameters) => {
                        action.Parameters = value;
                        //this.propagateChange([...this.actions])

                    }
                })
                break;
            }
        }

        this.close()
    }

    RemoveActions(actions: Array<MiddlerAction>) {

        console.log(actions);
        actions.forEach(act => this.actions = this.actions.filter(ac => ac != act));
        this.propagateChange(this.actions)
        this.close()
    }

    drop(event: CdkDragDrop<MiddlerAction[]>) {

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

    GetIcon(action: MiddlerAction) {

        return ActionHelper.GetIcon(action);
    }



    private propagateChange(value: Array<MiddlerAction>) {
        this.actionsChanged.next(value);
        this.registered.forEach(fn => {
            fn(value);
        });

    }

    ///ControlValueAccessor

    writeValue(obj: Array<MiddlerAction>): void {
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
