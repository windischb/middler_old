import { Component, OnInit, ViewContainerRef, ViewChild, TemplateRef, ChangeDetectionStrategy } from "@angular/core";
import { AppUIService } from '@services';
import { Router, ActivatedRoute } from '@angular/router';
import { IDPService } from '../idp.service';
import { MUserDto } from '../models/m-user-dto';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { IOverlayHandle, DoobOverlayService, DoobModalService } from '@doob-ng/cdk-helper';
import { tap, map } from 'rxjs/operators';
import { MUserListDto } from '../models/m-user-list-dto';
import { SetPasswordModalComponent } from './set-password-modal.component';
import { SetPasswordDto } from '../models/set-password-dto';
import { of } from 'rxjs';

@Component({
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UsersComponent {

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>


    users$ = this.idService.GetAllUsers().pipe(
        map(users => {
            console.log(users)
            return users.map(u => {

                let userLogins = u.Logins ?? [];

                if (u.HasPassword) {
                    userLogins = ["Local", ...userLogins]
                }

                return {
                    ...u,
                    Logins: userLogins
                }
            })
        })
    )

    grid = new GridBuilder<MUserListDto>()
        .SetColumns(
            c => c.Default("UserName").SetHeader("Username")
                .SetInitialWidth(200, true)
                .SetLeftFixed()
                .SetCssClass("pValue"),
            c => c.Default("FirstName").SetHeader("Firstname")
                .SetInitialWidth(200, true),
            c => c.Default("LastName").SetHeader("Lastname")
                .SetInitialWidth(200, true),
            c => c.Default("Email")
                .AddClassRule("not-confirmed", "!data.EmailConfirmed")
                .AddClassRule("confirmed1", "data.EmailConfirmed"),
            c => c.Default("PhoneNumber")
                .SetHeader("Phone")
                .AddClassRule("not-confirmed", "!data.PhoneNumberConfirmed")
                .AddClassRule("confirmed1", "data.PhoneNumberConfirmed"),
            c => c.Label("Logins")
                .SetInitialWidth(200, true)
                .SetRendererParams({
                    labelClass: () => "ant-tag"
                }),
        )
        .SetData(this.users$)
        .WithRowSelection("multiple")
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
        .SetRowClassRules({
            'deleted': 'data.Deleted'
        })
        .SetRowClassRules({
            'not-confirmed': '!data.Active'
        })
        .SetDataImmutable(data => data.Id);

    private contextMenu: IOverlayHandle;

    constructor(
        private uiService: AppUIService,
        private idService: IDPService,
        private router: Router,
        private route: ActivatedRoute,
        public overlay: DoobOverlayService,
        private modal: DoobModalService,
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



    SetPassword(user: MUserDto) {



        const modal = this.modal
            .FromComponent(SetPasswordModalComponent)
            .SetData({})
            .SetModalOptions({
                overlayConfig: {
                    width: "400px"
                }
            })
            .OnClose(() => {
                // this.toast.CloseToast(this.nameAlreadyExistsErrorToast)
                //this.nameAlreadyExistsErrorToast = null;
            })
            .AddEventHandler<SetPasswordDto>('SetPassword', (context) => {
                return this.idService.SetPassword(user.Id, context.payload);
            })
            .AddEventHandler<SetPasswordDto>('ClearPassword', (context) => {
                return this.idService.ClearPassword(user.Id);
            })
        // .AddEventHandler("changed", () => this.toast.CloseToast(this.nameAlreadyExistsErrorToast));


        modal.Open();
        this.contextMenu?.Close();
    }
}