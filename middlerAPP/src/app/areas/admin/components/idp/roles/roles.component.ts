import { Component, ViewChild, TemplateRef, ViewContainerRef } from "@angular/core";
import { AppUIService } from '@services';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { IDPService } from '../idp.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MRoleDto } from '../models/m-role-dto';
import { IdentityRolesQuery } from '../identity-roles.store';


@Component({
    templateUrl: './roles.component.html',
    styleUrls: ['./roles.component.scss']
})
export class RolesComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
   
    grid = new GridBuilder<MRoleDto>()
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
        .SetData(this.idService.GetAllRoles())
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
            this.EditRole(el.node.data);
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
        private identityRolesQuery: IdentityRolesQuery
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "IDP / Roles"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#user-tag"
        })

        // idService.GetAllRoles().subscribe(roles => {
        //     this.grid.SetData(roles);
        // });
    }

    AddRole() {
        this.router.navigate(["create"], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    EditRole(role: MRoleDto) {
        this.router.navigate([role.Id], { relativeTo: this.route });
        this.contextMenu?.Close();
    }

    RemoveRole(roles: Array<MRoleDto>) {
        this.idService.DeleteRole(...roles.map(r => r.Id)).subscribe();
        this.contextMenu?.Close();
    }

    ReloadRolesList() {
        this.idService.ReLoadRoles();
    }
}