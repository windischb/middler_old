import { Component, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { AppUIService } from '@services';
import { IMClientDto } from '../models/m-client-dto';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { IDPService } from '../idp.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './clients.component.html',
    styleUrls: ['./clients.component.scss']
})
export class ClientsComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>

    grid = new GridBuilder()
        .SetColumns(
            c => c.Default("")
                .SetMaxWidth(40)
                .SetMaxWidth(40)
                .SuppressSizeToFit()
                .Set(cl => {
                    cl.headerCheckboxSelection = true;
                    cl.checkboxSelection = true;
                }),
            c => c.Default("ClientId"),
            c => c.Default("ClientName")
        )
        .SetData(this.idService.GetAllClients())
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
            this.EditClient(el.node.data);
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        });

    private contextMenu: IOverlayHandle;


    constructor(
        private uiService: AppUIService,
        private idService: IDPService,
        private router: Router,
        private route: ActivatedRoute,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "IDP / Clients"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#desktop"
        })
    }

    AddClient() {
        this.router.navigate(["create"], { relativeTo: this.route })
    }

    EditClient(role: IMClientDto) {
        this.router.navigate([role.Id], { relativeTo: this.route })
    }

    RemoveClient(roles: Array<IMClientDto>) {

    }

    ReloadClientsList() {
        this.idService.ReLoadClients();
    }
}