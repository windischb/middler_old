import { Component, Input, Output, EventEmitter, TemplateRef, ViewContainerRef, ViewChild } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IOverlayHandle, DoobOverlayService, DoobModalService } from '@doob-ng/cdk-helper';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { Subject, BehaviorSubject, of } from 'rxjs';
import { Secret, SecretSortByExpiration } from '../models/secret';
import { SecretModalComponent } from './secret-modal.component';

@Component({
    selector: 'secrets-manager',
    templateUrl: './secrets-manager.component.html',
    styleUrls: ['./secrets-manager.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: SecretsManagerComponent,
        multi: true
    }],
})
export class SecretsManagerComponent implements ControlValueAccessor {


    private secretsSubject$ = new BehaviorSubject<Array<Secret>>(null);
    _secrets: Array<Secret> = [];
    sideBarOpen = false;
    @Input()
    set secrets(value: Array<Secret>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }
        this._secrets = cls.sort(SecretSortByExpiration);
        this.secretsSubject$.next(this._secrets)
        this.grid?.SetData(this._secrets);
    }
    get secrets() {
        return this._secrets;
    }

    private _disabled: boolean = false;
    @Input()
    get disabled() {
        return this._disabled;
    };
    set disabled(value: any) {
        if (value === null || value === undefined || value === false) {
            this._disabled = false
        } else {
            this._disabled = !!value
        }
    }



    @Output() secretsChange: EventEmitter<Array<Secret>> = new EventEmitter<Array<Secret>>();

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    // @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid = new GridBuilder<Secret>()
        .SetColumns(
            // c => c.Default("")
            //     .SetMaxWidth(40)
            //     .SetMaxWidth(40)
            //     .SuppressSizeToFit()
            //     .Set(cl => {
            //         cl.headerCheckboxSelection = true;
            //         cl.checkboxSelection = true;
            //     }),
            c => c.Default("Type").SetInitialWidth(160, true).Resizeable(false),
            c => c.Default("Description"),
            c => c.Date("Expiration").SetInitialWidth(200, true)
        )
        .WithRowSelection("multiple")
        .WithFullRowEditType()
        .WithShiftResizeMode()
        .OnDataUpdate(data => this.propagateChange(data))
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.node.setSelected(true, true)
            }

            let vContext = new DefaultContextMenuContext(ev.api, ev.event as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnViewPortContextMenu((ev, api) => {

            let vContext = new DefaultContextMenuContext(api, ev as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnRowDoubleClicked(el => {
            this.createSecret(el.data)
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => {
            ev.api.sizeColumnsToFit()
        })
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        //.SetDataImmutable(data => data.Id)
        .SetRowClassRules({
            'deleted': 'data.Deleted'
        })


    private contextMenu: IOverlayHandle;

    constructor(
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private modal: DoobModalService
    ) {

    }


    createSecret(secret?: Secret) {

        var variable = secret ?? new Secret();

        const modal = this.modal
            .FromComponent(SecretModalComponent)
            .SetData(variable)
            .SetMetaData({ create: !secret })
            .SetModalOptions({
                overlayConfig: {
                    width: "400px"
                }
            })
            .OnClose(() => {
                // this.toast.CloseToast(this.nameAlreadyExistsErrorToast)
                //this.nameAlreadyExistsErrorToast = null;
            })
            .AddEventHandler<Secret>('ok', (context) => {
                // this.toast.CloseToast(this.nameAlreadyExistsErrorToast);
                let v = context.payload
                v.Id = secret?.Id;
                if(!secret) {
                    this.secrets = [...this.secrets, v]
                } else {
                    this.secrets = this.secrets.map(s => {
                        if(s == secret) {
                            return v;
                        } else {
                            return s;
                        }
                    })
                }
                
                console.log(v);
                return of(true);
            })
        // .AddEventHandler("changed", () => this.toast.CloseToast(this.nameAlreadyExistsErrorToast));


        modal.Open();
        this.contextMenu?.Close();
    }



    RemoveSecret(arr: Array<Secret>): void {
        //let ids = arr.map(c => c.Id);
        this.secrets = this.secrets.filter(d => !arr.includes(d));
        this.contextMenu.Close();
    }

    AddSecret() {
        // this.openTemplate();
        this.createSecret();
        this.contextMenu.Close();
    }

    SecretsChanged(nSecrets: Array<Secret>) {
        this.secrets = nSecrets;
        this.propagateChange(nSecrets)
    }

    propagateChange(value: Array<Secret>) {

        this.secretsChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<Secret>): void {
        this.secrets = value;
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
        this.disabled = isDisabled;
    }

}

