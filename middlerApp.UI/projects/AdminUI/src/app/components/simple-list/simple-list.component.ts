import { Component, Input, Output, EventEmitter, TemplateRef, ViewContainerRef, ViewChild, OnInit, ElementRef } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { Subject, BehaviorSubject } from 'rxjs';
import { CellEditingStoppedEvent } from '@ag-grid-community/all-modules';

@Component({
    selector: 'simple-list',
    templateUrl: './simple-list.component.html',
    styleUrls: ['./simple-list.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: SimpleListComponent,
        multi: true
    }],
})
export class SimpleListComponent implements ControlValueAccessor, OnInit {


    _entries: Array<ListItem> = [];

    @Input()
    set entries(value: Array<string>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }
        this.SetListEntries(cls.map(v => new ListItem(v)));

    }
    get entries() {
        return this._entries.map(e => e.Value);
    }

    private SetListEntries(value: Array<ListItem>) {
        this._entries = value;
        this.grid?.SetData(this._entries);
    }

    @Input() header: string;

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


    private _noScrollbar: boolean = false;
    @Input("no-scrollbar")
    get noScrollbar() {
        return this._noScrollbar;
    };
    set noScrollbar(value: any) {
        if (value === null || value === undefined || value === false) {
            this._noScrollbar = false
        } else {
            this._noScrollbar = !!value
        }
    }

    @Input() checkboxSelection: boolean = false;

    @Output() entriesChange: EventEmitter<Array<string>> = new EventEmitter<Array<string>>();

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    // @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid: GridBuilder;

    private contextMenu: IOverlayHandle;

    constructor(
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private element: ElementRef
    ) {

    }

    ngOnInit() {

        var el = this.element.nativeElement as HTMLElement;
        if (el.attributes.getNamedItem("disabled")) {
            this.disabled = true;
        }

        if (el.attributes.getNamedItem("no-scrollbar")) {
            this.noScrollbar = true;
        }

        const builder = new GridBuilder();

        if (this.checkboxSelection) {
            builder.SetColumns(
                c => c.Default("")
                    .SetMaxWidth(40)
                    .SetMaxWidth(40)
                    .SuppressSizeToFit()
                    .Set(cl => {
                        cl.headerCheckboxSelection = this.checkboxSelection;
                        cl.checkboxSelection = this.checkboxSelection;
                    })
            )
        }

        this.grid = builder.SetColumns(c => c.Default("Value").SetHeader(this.header).Editable())
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
            .SetData(this.entries)
            .StopEditingWhenGridLosesFocus()
            .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
            .OnViewPortClick((ev, api) => {
                api.deselectAll();
            })
            .SetGridOptions({
                onCellEditingStopped: (options: CellEditingStoppedEvent) => {
                    let items: Array<any> = [];
                    options.api.forEachNode(function (node) {
                        items.push(node.data);
                    });


                    this.propagateChange(items)
                }
            });
    }


    AddEntry(): void {
        this.entries = [
            ...this.entries,
            null
        ];
        this.contextMenu.Close();

    }

    RemoveEntry(arr: Array<ListItem>): void {

        this.SetListEntries(this._entries.filter(d => !arr.includes(d)));
        this.contextMenu.Close();
    }



    propagateChange(value: Array<ListItem>) {

        const arr = value.map(e => e.Value).filter(Boolean);
        this.entriesChange.emit(arr);
        this.registered.forEach(fn => {
            fn(arr);
        });
    }

    writeValue(value: Array<string>): void {
        this.entries = value;
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

export class ListItem {
    Value: any;

    constructor(value?: any) {
        this.Value = value;
    }
}

