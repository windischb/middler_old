import { Component, OnInit, ViewContainerRef, ViewChild, TemplateRef, ChangeDetectionStrategy } from "@angular/core";
import { AppUIService } from '@services';
import { Router, ActivatedRoute } from '@angular/router';
import { IDPService } from '../idp.service';
import { MUserDto } from '../models/m-user-dto';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { tap } from 'rxjs/operators';
import { MUserListDto } from '../models/m-user-list-dto';

@Component({
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UsersComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>

    grid = new GridBuilder<MUserListDto>()
        .SetColumns(
            c => c.Default("")
                .SetMaxWidth(40)
                .SetMaxWidth(40)
                .SuppressSizeToFit()
                .Set(cl => {
                    cl.headerCheckboxSelection = true;
                    cl.checkboxSelection = true;
                }),
            c => c.Default("UserName")
                .SetInitialWidth(200, true),
            c => c.Default("FirstName").SetInitialWidth(200, true),
            c => c.Default("LastName").SetInitialWidth(200, true),
            c => c.Default("Email")
        )
        .SetData(this.idService.GetAllUsers())
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
            this.EditUser(el.node.data);
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        .SetDataImmutable(data => data.Id);

    private contextMenu: IOverlayHandle;

    constructor(
        private uiService: AppUIService,
        private idService: IDPService,
        private router: Router,
        private route: ActivatedRoute,
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef) {
        uiService.Set(ui => {
            ui.Header.Title = "IDP / Users"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#user"
        })
    }


    AddUser() {
        this.router.navigate(["create"], { relativeTo: this.route })
    }

    EditUser(user: MUserDto) {
        this.router.navigate([user.Id], { relativeTo: this.route })
    }

    RemoveUser(users: Array<MUserDto>) {
        this.idService.DeleteUser(...users.map(u => u.Id)).subscribe()
    }

    ReloadUsersList() {
        this.idService.ReLoadUsers();
    }
}