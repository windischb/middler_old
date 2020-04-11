import { Component, Input, TemplateRef, ViewContainerRef, ViewChild } from "@angular/core";
import { VariableInfo } from '../../models/variable-info';
import { DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { GridBuilder } from "@doob-ng/grid";
import { GridApi } from '@ag-grid-community/all-modules';
import { VariablesService } from '../../services/variables.service';
import { Variable } from '../../models/variable';
import { VariableModalService } from '../../services/variable-modal.service';
import { LanguageHelper } from "@doob-ng/editor";
import { catchError, tap } from 'rxjs/operators';
import { DoobToastService } from '@doob-ng/ui';

@Component({
    selector: 'variables-folder-content',
    templateUrl: './folder-content.component.html',
    styleUrls: ['./folder-content.component.scss'],
    host: {
        class: ''
    },
    providers: [
        VariableModalService
    ]
})
export class VariablesFolderContentComponent {

    @Input()
    set Items(value: Array<VariableInfo>) {
        this.grid?.SetData(value);
    }

    @Input() Path: string;

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid = new GridBuilder()
        .SetColumns(
            c => c.Default("Name"),
            c => c.Label("Extension").SetHeader("Type").SetValueFormatter<string>(v => v.value.slice(1)),
            c => c.Date("CreatedAt", true),
            c => c.Date("UpdatedAt", true)
        )
        .SetGridOptions({
            rowSelection: 'multiple',
            colResizeDefault: 'shift'
        })
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.api.selectNode(ev.node);
            }

            let vContext = new VaribaleItemsContext(ev.api)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnViewPortContextMenu((ev, api) => {
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.viewportContextMenu, this.viewContainerRef, {})
        })
        .OnRowDoubleClicked(el => {
            this.OpenEditVariableModal(el.data)
        });

    private contextMenu: IOverlayHandle;

    constructor(
        private overlay: DoobOverlayService,
        private viewContainerRef: ViewContainerRef,
        private variables: VariablesService,
        private toast: DoobToastService,
        private modalService: VariableModalService) {

    }


    OpenEditVariableModal(variable: Variable, raw: boolean = false) {


        this.variables.GetVariable(variable.Parent, variable.Name).subscribe(v => {

            const modal = this.modalService
                .GetModal(raw ? 'raw' : v.Extension)
                .SetData(v)
                .SetMetaData({
                    raw: raw
                })
                .AddEventHandler<Variable>('ok', (context) => {

                    if (raw) {
                        context.payload.Extension = variable.Extension
                    }
                    return this.variables.UpdateVariableContent(`${v.Parent}/${v.Name}`, context.payload.Content).pipe(
                        catchError(err => {

                            this.nameAlreadyExistsErrorToast = this.toast.AddError({ title: "Error", message: err.message }, { displayTime: 0 })

                            throw err;
                        }),
                        tap(c => this.toast.AddSuccess({ title: "Success", message: `'${context.payload.Name}' updated` }, { displayTime: 2000 }))
                    )
                });


            modal.Open();

        })


    }

    nameAlreadyExistsErrorToast: any;
    createVariable(extension: string) {

        var variable = new Variable();
        variable.Parent = this.Path;

        const modal = this.modalService
            .GetModal(extension)
            .SetData(variable)
            .SetMetaData({ create: true })
            .OnClose(() => {
                this.toast.CloseToast(this.nameAlreadyExistsErrorToast)
                this.nameAlreadyExistsErrorToast = null;
            })
            .AddEventHandler<Variable>('ok', (context) => {
                this.toast.CloseToast(this.nameAlreadyExistsErrorToast);
                let v = context.payload
                v.Parent = this.Path;

                if (v.Extension) {
                    if (!v.Extension.startsWith('.')) {
                        v.Extension = LanguageHelper.GetDefaultFileExtension(v.Extension);
                    }
                } else {
                    v.Extension = extension;
                }

                return this.variables.CreateVariable(v).pipe(
                    catchError(err => {

                        this.nameAlreadyExistsErrorToast = this.toast.AddError({ title: "Error", message: err.message }, { displayTime: 0 })

                        throw err;
                    }),
                    tap(c => this.toast.AddSuccess({ title: "Success", message: `'${context.payload.Name}' created` }, { displayTime: 2000 }))
                )
            })
            .AddEventHandler("changed", () => this.toast.CloseToast(this.nameAlreadyExistsErrorToast));


        modal.Open();
        this.contextMenu?.Close();
    }

    RemoveVariable(variables: Array<Variable>) {
        variables.forEach(variable => {
            this.variables.RemoveVariable(variable.FullPath).subscribe();
        })
        this.contextMenu.Close()
    }

}

export class VaribaleItemsContext<T = any> {

    SelectedData: Array<T>;

    get SelectedCount() {
        return this.SelectedData.length;
    }

    get Any() {
        return this.SelectedCount > 0;
    }
    get Single() {
        return this.SelectedCount === 1;
    }

    get First() {
        return this.SelectedCount > 0 ? this.SelectedData[0] : null;
    }

    constructor(gridApi: GridApi) {
        this.SelectedData = gridApi.getSelectedNodes().map(n => n.data)
    }

}
