import { Component, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { AppUIService } from '@services';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { IDPService } from '../idp.service';
import { Router, ActivatedRoute } from '@angular/router';
import { tap, takeUntil } from 'rxjs/operators';

import { ApiScopesQuery } from './api-scopes.store';
import { IMScopeDto } from '../models/m-scope-dto';

@Component({
    templateUrl: './api-scopes.component.html',
    styleUrls: ['./api-scopes.component.scss']
})
export class ApiScopesComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>

    grid = new GridBuilder<IMScopeDto>()
        .SetColumns(
            c => c.Default("Name")
                .SetInitialWidth(200, true)
                .SetLeftFixed()
                .SetCssClass("pValue"),
            c => c.Default("DisplayName"),
            c => c.Default("Description")
        )
        .SetData(this.idService.GetAllApiScopes())
        .WithRowSelection("multiple")
        .WithFullRowEditType()
        .WithShiftResizeMode()
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.node.setSelected(true, true)
            }

            let vContext = new DefaultContextMenuContext(ev.api, ev.event as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnViewPortContextMenu((ev, api) => {
            let vContext = new DefaultContextMenuContext(api, ev)
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnRowDoubleClicked(el => {
            this.EditApiScope(el.node.data);
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        .SetRowClassRules({
            'deleted': 'data.Deleted'
        })
        .SetDataImmutable(data => data.Id);



    private contextMenu: IOverlayHandle;

    constructor(
        private uiService: AppUIService,
        private idService: IDPService,
        private router: Router,
        private route: ActivatedRoute,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private apiResourcesQuery: ApiScopesQuery
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "IDP / Api-Scopes"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#cube"
        })

        // idService.GetAllApiScopes().subscribe(api-resources => {
        //     this.grid.SetData(api-resources);
        // });
    }

    AddApiScope() {
        this.router.navigate(["create"], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    EditApiScope(item: IMScopeDto) {
        this.router.navigate([item.Id], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    RemoveApiScope(item: Array<IMScopeDto>) {
        this.idService.DeleteApiScope(...item.map(r => r.Id)).subscribe();
        this.contextMenu?.Close();
    }

    ReloadApiScopesList() {
        this.idService.ReLoadApiScopes();
    }
}