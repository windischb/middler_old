import { Component, Input, Output, EventEmitter, TemplateRef, ViewContainerRef, ViewChild } from "@angular/core";
import { SimpleClaim } from './simple-claim';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';

@Component({
    selector: 'claims-manager',
    templateUrl: './claims-manager.component.html',
    styleUrls: ['./claims-manager.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: ClaimsManagerComponent,
        multi: true
    }],
})
export class ClaimsManagerComponent implements ControlValueAccessor {


    _claims: Array<SimpleClaim> = [];

    @Input()
    set claims(value: Array<SimpleClaim>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }
        this._claims = cls;
        this.grid?.SetData(this._claims);
    }
    get claims() {
        return this._claims;
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

    @Output() claimsChange: EventEmitter<Array<SimpleClaim>> = new EventEmitter<Array<SimpleClaim>>();

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    // @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid = new GridBuilder()
        .SetColumns(
            c=>c.Default("")
            .SetMaxWidth(40)
            .SetMaxWidth(40)
            .SuppressSizeToFit()
            .Set(cl => {
                cl.headerCheckboxSelection = true;
                cl.checkboxSelection = true;
            }),
            c => c.Default("Type")
                .Editable()
                .SetInitialWidth(300, true)
                .Set(cl => {
                    cl.filter = 'agTextColumnFilter';
                    cl.filterParams = {
                        buttons: ['reset', 'apply'],
                    }
                }),
            c => c.Default("Value").Editable()
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
            api.deselectAll();
            let vContext = new DefaultContextMenuContext(api, ev as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnRowDoubleClicked(el => {
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        });

    private contextMenu: IOverlayHandle;

    constructor(
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef
        ) {

    }


    private i: number = 0;
    private buildNextClaim() {
        var cl = new SimpleClaim();
        this.i++;
        return cl;
    }

    AddClaim(): void {
        this.claims = [
            ...this.claims,
            this.buildNextClaim()
        ];
        this.contextMenu.Close();

    }

    RemoveClaim(arr: Array<SimpleClaim>): void {
        //let ids = arr.map(c => c.Id);
        this.claims = this.claims.filter(d => !arr.includes(d));
        this.contextMenu.Close();
    }



    propagateChange(value: Array<SimpleClaim>) {

        this.claimsChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<SimpleClaim>): void {
        this.claims = value;
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

// export class ClaimsItemsContext<T = any> {

//     SelectedData: Array<T>;

//     get SelectedCount() {
//         return this.SelectedData.length;
//     }

//     get Any() {
//         return this.SelectedCount > 0;
//     }
//     get Single() {
//         return this.SelectedCount === 1;
//     }

//     get First() {
//         return this.SelectedCount > 0 ? this.SelectedData[0] : null;
//     }

//     constructor(gridApi: GridApi, private mEvent: MouseEvent) {
//         this.SelectedData = gridApi.getSelectedNodes().map(n => n.data)
//     }

//     get IsShiftPressed() {
//         return this.mEvent.shiftKey;
//     }

// }