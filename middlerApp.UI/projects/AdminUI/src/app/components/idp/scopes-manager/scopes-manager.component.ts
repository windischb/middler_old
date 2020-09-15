import { Component, ChangeDetectionStrategy, Input, Output, EventEmitter, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { BehaviorSubject, combineLatest } from 'rxjs';
import { IDPService } from '../idp.service';
import { map, tap } from 'rxjs/operators';
import { IMScopeDto } from '../models/m-scope-dto';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { ScopeGridCellComponent } from './scope-grid-cell.component';
import { DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { IconGridCellComponent } from '../shared/components/icon-cell.component';

@Component({
    selector: 'scopes-manager',
    templateUrl: './scopes-manager.component.html',
    styleUrls: ['./scopes-manager.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: ScopesManagerComponent,
        multi: true
    }],
})
export class ScopesManagerComponent implements ControlValueAccessor {

    private assignedScopesSubject$ = new BehaviorSubject<Array<IMScopeDto>>([]);

    @Input()
    set assignedScopes(value: Array<IMScopeDto>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }

        this.assignedScopesSubject$.next(value)

    }
    get assignedScopes() {
        return this.assignedScopesSubject$.value;
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

    @Output() assignedScopesChange: EventEmitter<Array<IMScopeDto>> = new EventEmitter<Array<IMScopeDto>>();

    availableScopes$ = combineLatest(this.idService.GetAllApiScopes(), this.idService.GetAllIdentityResources())
        .pipe(
            map(([apiResourceScopes, identityResourceScopes]) => {
                return <Array<IMScopeDto>>[...apiResourceScopes, ...identityResourceScopes].sort((a, b) => {

                    if (a.Name > b.Name) {
                        return 1;
                    }

                    if (a.Name < b.Name) {
                        return -1;
                    }

                    return 0;
                });
            }),
            map(scopes => {
                return scopes.map(s => this.NormalizeScopeProperties(s))
            })
        );

    filteredAvailableScopes$ = combineLatest(this.availableScopes$, this.assignedScopesSubject$).pipe(
        map(([available, assigned]) => {

            return available.filter(av => assigned.map(ass => ass.Id).indexOf(av.Id) === -1);
        })
    );

    normalizedAssignedScopes$ = this.assignedScopesSubject$.pipe(
        map(scopes => {
            return <Array<IMScopeDto>>scopes.sort((a, b) => {

                if (a.Name > b.Name) {
                    return 1;
                }

                if (a.Name < b.Name) {
                    return -1;
                }

                return 0;
            });
        }),
        map(scopes => {
            return scopes.map(s => this.NormalizeScopeProperties(s))
        })
    )



    grid = new GridBuilder<IMScopeDto>()
        .SetColumns(
            c => c.Default("Type")
                .SetRenderer(IconGridCellComponent)
                .SetCssClass("scope-type")
                .SetFixedWidth(50),
            c => c.Default("Name")
                .SetRenderer(ScopeGridCellComponent)
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
        .SetData(this.normalizedAssignedScopes$)
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

    availablegrid = new GridBuilder<IMScopeDto>()
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
                .SetFixedWidth(50),
            c => c.Default("Name")
                .SetRenderer(ScopeGridCellComponent)
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
        .SetData(this.filteredAvailableScopes$)
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


    private NormalizeScopeProperties(scope: IMScopeDto) {

        let icon = scope.Type;
        switch (icon) {
            case "ApiScope": {
                icon = "fa#cube";
                break;
            }
            case "IdentityResource": {
                icon = "fa#far|address-card";
                break;
            }
        }

        return {
            ...scope,
            Type: icon
        }

    }

    Remove(arr: Array<IMScopeDto>): void {
        this.assignedScopes = this.grid.GetData().filter(d => !arr.map(a => a.Id).includes(d.Id));
        this.contextMenu?.Close();
    }

    Add(arr: Array<IMScopeDto>): void {
        this.assignedScopes = [...this.assignedScopes, ...arr.filter(a => !this.assignedScopes.map(s => s.Id).includes(a.Id))]
        this.contextMenu?.Close();
    }

    propagateChange(value: Array<IMScopeDto>) {

        this.assignedScopesChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<IMScopeDto>): void {
        this.assignedScopes = value;
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