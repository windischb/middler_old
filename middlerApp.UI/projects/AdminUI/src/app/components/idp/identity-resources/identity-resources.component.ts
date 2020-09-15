import { Component, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { AppUIService } from '@services';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { IDPService } from '../idp.service';
import { Router, ActivatedRoute } from '@angular/router';
import { tap, takeUntil } from 'rxjs/operators';

import { Subject, BehaviorSubject } from 'rxjs';
import { IMIdentityResourceListDto } from '../models/m-identity-resource-list-dto';
import { IdentityResourcesQuery } from './identity-resources.store';

@Component({
    templateUrl: './identity-resources.component.html',
    styleUrls: ['./identity-resources.component.scss']
})
export class IdentityResourcesComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>

    grid = new GridBuilder<IMIdentityResourceListDto>()
        .SetColumns(
            c => c.Default("Name")
                .SetInitialWidth(200, true)
                .SetLeftFixed()
                .SetCssClass("pValue"),
            c => c.Default("DisplayName"),
            c => c.Default("Description")
        )
        .SetData(this.idService.GetAllIdentityResources())
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
        .OnViewPortContextMenu((ev, identity) => {
            let vContext = new DefaultContextMenuContext(identity, ev)
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnRowDoubleClicked(el => {
            this.EditIdentityResource(el.node.data);
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, identity) => {
            identity.deselectAll();
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
        private identityResourcesQuery: IdentityResourcesQuery
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "IDP / Identity-Resources"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#far|address-card"
        })

        // idService.GetAllIdentityResources().subscribe(identity-resources => {
        //     this.grid.SetData(identity-resources);
        // });
    }

    AddIdentityResource() {
        this.router.navigate(["create"], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    EditIdentityResource(item: IMIdentityResourceListDto) {
        this.router.navigate([item.Id], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    RemoveIdentityResource(item: Array<IMIdentityResourceListDto>) {
        this.idService.DeleteIdentityResource(...item.map(r => r.Id)).subscribe();
        this.contextMenu?.Close();
    }

    ReloadIdentityResourcesList() {
        this.idService.ReLoadIdentityResources();
    }
}