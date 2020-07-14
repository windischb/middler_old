import { Component, Input, Output, EventEmitter, TemplateRef, ViewContainerRef, ViewChild } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { GridApi, GridSizeChangedEvent } from '@ag-grid-community/all-modules';
import { MRoleDto, MRoleDtoSortByName } from '../models/m-role-dto';
import { NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { Subject, BehaviorSubject } from 'rxjs';

@Component({
    selector: 'roles-manager',
    templateUrl: './roles-manager.component.html',
    styleUrls: ['./roles-manager.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: RolesManagerComponent,
        multi: true
    }],
})
export class RolesManagerComponent implements ControlValueAccessor {


    private rolesSubject$ = new BehaviorSubject<Array<MRoleDto>>(null);
    _roles: Array<MRoleDto> = [];
    sideBarOpen = false;
    @Input()
    set roles(value: Array<MRoleDto>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }
        this._roles = cls.sort(MRoleDtoSortByName);
        this.rolesSubject$.next(this._roles)
        this.grid?.SetData(this._roles);
    }
    get roles() {
        return this._roles;
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

    

    @Output() rolesChange: EventEmitter<Array<MRoleDto>> = new EventEmitter<Array<MRoleDto>>();

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    // @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid = new GridBuilder<MRoleDto>()
        .SetColumns(
            c => c.Default("")
                .SetMaxWidth(40)
                .SetMaxWidth(40)
                .SuppressSizeToFit()
                .Set(cl => {
                    cl.headerCheckboxSelection = true;
                    cl.checkboxSelection = true;
                }),
            c => c.Default("Name")
                .SetInitialWidth(300, true),
            c => c.Default("DisplayName")
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
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        .SetDataImmutable(data => data.Id)
        .SetRowClassRules({
            'deleted': 'data.Deleted'
        });

    private contextMenu: IOverlayHandle;

    constructor(
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private drawerService: NzDrawerService
    ) {

    }


    RemoveRole(arr: Array<MRoleDto>): void {
        //let ids = arr.map(c => c.Id);
        this.roles = this.roles.filter(d => !arr.includes(d));
        this.contextMenu.Close();
    }

    AddRole() {
        // this.openTemplate();
        this.sideBarOpen = true;
        this.contextMenu.Close();
    }

    RolesChanged(nRoles: Array<MRoleDto>) {
        this.roles = nRoles;
        this.propagateChange(nRoles)
    }


    @ViewChild('drawerTemplate', { static: false }) drawerTemplate?: TemplateRef<{
        $implicit: { value: string };
        drawerRef: NzDrawerRef<string>;
    }>;

    value = 'ng';

    openTemplate(): void {
        const drawerRef = this.drawerService.create({
            nzTitle: 'Template',
            nzContent: this.drawerTemplate,
            nzContentParams: {
                value: this.value
            },
            nzMask: false,
            nzWidth: 400
        });

        drawerRef.afterOpen.subscribe(() => {
            console.log('Drawer(Template) open');
        });

        drawerRef.afterClose.subscribe(() => {
            console.log('Drawer(Template) close');
        });
    }


    propagateChange(value: Array<MRoleDto>) {

        this.rolesChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<MRoleDto>): void {
        this.roles = value;
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

