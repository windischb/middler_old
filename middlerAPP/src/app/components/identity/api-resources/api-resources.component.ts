import { Component, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { AppUIService } from '@services';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { IdentityService } from '../identity.service';
import { Router, ActivatedRoute } from '@angular/router';
import { tap, takeUntil } from 'rxjs/operators';

import { Subject, BehaviorSubject } from 'rxjs';
import { IMApiResourceListDto } from '../models/m-api-resource-list-dto';
import { IdentityApiResourcesQuery } from './identity-api-resources.store';

@Component({
    templateUrl: './api-resources.component.html',
    styleUrls: ['./api-resources.component.scss']
})
export class ApiResourcesComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
   
    grid = new GridBuilder<IMApiResourceListDto>()
        .SetColumns(
            c => c.Default("")
                .SetMaxWidth(40)
                .SetMaxWidth(40)
                .SuppressSizeToFit()
                .Set(cl => {
                    cl.headerCheckboxSelection = true;
                    cl.checkboxSelection = true;
                    cl.headerCheckboxSelectionFilteredOnly = true
                }),
            c => c.Default("Name"),
            c => c.Default("DisplayName"),
            c => c.Default("Description")
        )
        .SetData(this.idService.GetAllApiResources())
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
            this.EditApiResource(el.node.data);
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
        private idService: IdentityService,
        private router: Router,
        private route: ActivatedRoute,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private identityApiResourcesQuery: IdentityApiResourcesQuery
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "Identity / ApiResources"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#cubes"
        })

        // idService.GetAllApiResources().subscribe(api-resources => {
        //     this.grid.SetData(api-resources);
        // });
    }

    AddApiResource() {
        this.router.navigate(["create"], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    EditApiResource(item: IMApiResourceListDto) {
        this.router.navigate([item.Id], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    RemoveApiResource(item: Array<IMApiResourceListDto>) {
        this.idService.DeleteApiResource(...item.map(r => r.Id)).subscribe();
        this.contextMenu?.Close();
    }

    ReloadApiResourcesList() {
        this.idService.ReLoadApiResources();
    }
}