import { Component, ChangeDetectionStrategy, Input, Output, EventEmitter, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { IDPService } from '../idp.service';
import { map, tap } from 'rxjs/operators';
import { MRoleDto } from '../models/m-role-dto';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { RoleGridCellComponent } from './role-grid-cell.component';
import { DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { IconGridCellComponent } from '../shared/components/icon-cell.component';

@Component({
    selector: 'roles-manager',
    templateUrl: './roles-manager.component.html',
    styleUrls: ['./roles-manager.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: RolesManagerComponent,
        multi: true
    }],
})
export class RolesManagerComponent implements ControlValueAccessor {

    private assignedRolesSubject$ = new BehaviorSubject<Array<MRoleDto>>([]);

    @Input()
    set assignedRoles(value: Array<MRoleDto>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }

        this.assignedRolesSubject$.next(value)

    }
    get assignedRoles() {
        return this.assignedRolesSubject$.value;
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

    @Output() assignedRolesChange: EventEmitter<Array<MRoleDto>> = new EventEmitter<Array<MRoleDto>>();

    availableRoles$ = this.idService.GetAllRoles()
        .pipe(
            map((apiResourceRoles) => {
                return <Array<MRoleDto>>apiResourceRoles.sort((a, b) => {

                    if (a.Name > b.Name) {
                        return 1;
                    }

                    if (a.Name < b.Name) {
                        return -1;
                    }

                    return 0;
                });
            }),
            // map(roles => {
            //     return roles.map(s => this.NormalizeRoleProperties(s))
            // })
        );

    filteredAvailableRoles$ = combineLatest(this.availableRoles$, this.assignedRolesSubject$).pipe(
        map(([available, assigned]) => {

            return available.filter(av => assigned.map(ass => ass.Id).indexOf(av.Id) === -1);
        })
    );

    normalizedAssignedRoles$ = this.assignedRolesSubject$.pipe(
        map(roles => {
            return <Array<MRoleDto>>roles.sort((a, b) => {

                if (a.Name > b.Name) {
                    return 1;
                }

                if (a.Name < b.Name) {
                    return -1;
                }

                return 0;
            });
        }),
        // map(roles => {
        //     return roles.map(s => this.NormalizeRoleProperties(s))
        // })
    )



    grid = new GridBuilder<MRoleDto>()
        .SetColumns(
            c => c.Default("Type")
                .SetRenderer(IconGridCellComponent)
                .SetCssClass("scope-type")
                .SetRendererParams({
                    icon: "fa#user-tag"
                })
                .SetFixedWidth(50),
            c => c.Default("Name")
                .SetRenderer(RoleGridCellComponent)
                .Set(col => {
                    col.autoHeight = true;
                }),
            c => c.Default("")
                .SetCssClass("action-column")
                .SetRenderer(IconGridCellComponent)
                .SetRendererParams({
                    icon: "delete",
                    onClick: (ev, node) => {
                        const api = this.grid.GetGridApi();
                        const selected = this.grid.GetSelectedData();
                        if (!selected || selected.length <= 1) {
                            this.Remove([node.data])
                        } else {
                            this.Remove(selected)
                        }
                        api.deselectAll()

                    }
                })
                .SetFixedWidth(50)
        )
        .WithRowSelection("multiple")
        .OnDataUpdate(data => {
            this.propagateChange(data)
        })
        .SetData(this.normalizedAssignedRoles$)
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.node.setSelected(true, true)
            }

            let vContext = new DefaultContextMenuContext(ev.api, ev.event as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        .SetDataImmutable(data => data.Id);

    availablegrid = new GridBuilder<MRoleDto>()
        .SetColumns(
            c => c.Default("")
                .SetRenderer(IconGridCellComponent)
                .SetCssClass("action-column")
                .SetRendererParams({
                    icon: "left",
                    onClick: (ev, node) => {
                        const api = this.availablegrid.GetGridApi();
                        const selected = api.getSelectedNodes().map(n => n.data)
                        if (!selected || selected.length <= 1) {
                            this.Add([node.data])
                        } else {
                            this.Add(selected)
                        }
                        api.deselectAll()

                    }
                })
                .SetFixedWidth(50),
            c => c.Default("Type")
                .SetRenderer(IconGridCellComponent)
                .SetCssClass("scope-type")
                .SetRendererParams({
                    icon: "fa#user-tag"
                })
                .SetFixedWidth(50),
            c => c.Default("Name")
                .SetRenderer(RoleGridCellComponent)
                .Set(col => {
                    col.autoHeight = true;
                })
        )
        .WithRowSelection("multiple")
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.node.setSelected(true, true)
            }

            let vContext = new DefaultContextMenuContext(ev.api, ev.event as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.availableContextMenu, this.viewContainerRef, vContext)
        })
        .SetData(this.filteredAvailableRoles$)
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        }).SetDataImmutable(data => data.Id);

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    @ViewChild('availableContextMenu') availableContextMenu: TemplateRef<any>
    private contextMenu: IOverlayHandle;

    constructor(
        private idService: IDPService,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef) {

    }


    // private NormalizeRoleProperties(role: MRoleDto) {

    //     let icon = role.Type;
    //     switch (icon) {
    //         case "ApiRole": {
    //             icon = "fa#cube";
    //             break;
    //         }
    //         case "IdentityResource": {
    //             icon = "fa#far|address-card";
    //             break;
    //         }
    //     }

    //     return {
    //         ...role,
    //         Type: icon
    //     }

    // }

    Remove(arr: Array<MRoleDto>): void {
        this.assignedRoles = this.grid.GetData().filter(d => !arr.map(a => a.Id).includes(d.Id));
        this.contextMenu?.Close();
    }

    Add(arr: Array<MRoleDto>): void {
        this.assignedRoles = [...this.assignedRoles, ...arr.filter(a => !this.assignedRoles.map(s => s.Id).includes(a.Id))]
        this.contextMenu?.Close();
    }

    propagateChange(value: Array<MRoleDto>) {

        this.assignedRolesChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<MRoleDto>): void {
        this.assignedRoles = value;
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