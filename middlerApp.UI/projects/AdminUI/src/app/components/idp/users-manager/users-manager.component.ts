import { Component, Input, Output, EventEmitter, TemplateRef, ViewContainerRef, ViewChild } from "@angular/core";
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { IOverlayHandle, DoobOverlayService } from '@doob-ng/cdk-helper';
import { GridBuilder, DefaultContextMenuContext } from '@doob-ng/grid';
import { NzDrawerRef, NzDrawerService } from 'ng-zorro-antd/drawer';
import { Subject, BehaviorSubject } from 'rxjs';
import { MUserListDto, MUserListDtoSortByName } from '../models/m-user-list-dto';

@Component({
    selector: 'users-manager',
    templateUrl: './users-manager.component.html',
    styleUrls: ['./users-manager.component.scss'],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: UsersManagerComponent,
        multi: true
    }],
})
export class UsersManagerComponent implements ControlValueAccessor {


    private usersSubject$ = new BehaviorSubject<Array<MUserListDto>>(null);
    _users: Array<MUserListDto> = [];
    sideBarOpen = false;
    @Input()
    set users(value: Array<MUserListDto>) {
        let cls = value || [];
        if (cls.length == 0) {
            cls = [];
        }
        this._users = cls.sort(MUserListDtoSortByName);
        this.usersSubject$.next(this._users)
        this.grid?.SetData(this._users);
    }
    get users() {
        return this._users;
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

    

    @Output() usersChange: EventEmitter<Array<MUserListDto>> = new EventEmitter<Array<MUserListDto>>();

    @ViewChild('itemsContextMenu') itemsContextMenu: TemplateRef<any>
    // @ViewChild('viewportContextMenu') viewportContextMenu: TemplateRef<any>

    grid = new GridBuilder<MUserListDto>()
        .SetColumns(
            c => c.Default("UserName")
                .SetInitialWidth(200, true),
            c => c.Default("FirstName").SetInitialWidth(200, true),
            c => c.Default("LastName").SetInitialWidth(200, true),
            c => c.Default("Email")
        )
        .WithRowSelection("multiple")
        .WithFullRowEditType()
        .WithShiftResizeMode()
        .OnDataUpdate(data => this.propagateChange(data))
        .OnCellContextMenu(ev => {
            const selected = ev.api.getSelectedNodes();
            if (selected.length == 0 || !selected.includes(ev.node)) {
                ev.node.setSelected(true, true)
            }

            let vContext = new DefaultContextMenuContext(ev.api, ev.event as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev.event as MouseEvent, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnViewPortContextMenu((ev, api) => {
            
            let vContext = new DefaultContextMenuContext(api, ev as MouseEvent)
            this.contextMenu = this.overlay.OpenContextMenu(ev, this.itemsContextMenu, this.viewContainerRef, vContext)
        })
        .OnRowDoubleClicked(el => {
            //console.log("double Clicked", el)

        })
        .StopEditingWhenGridLosesFocus()
        .OnGridSizeChange(ev => ev.api.sizeColumnsToFit())
        .OnViewPortClick((ev, api) => {
            api.deselectAll();
        })
        .SetDataImmutable(data => data.Id)
        .SetRowClassRules({
            'deleted': 'data.Deleted'
        });

    private contextMenu: IOverlayHandle;

    constructor(
        public overlay: DoobOverlayService,
        public viewContainerRef: ViewContainerRef,
        private drawerService: NzDrawerService
    ) {

    }


    RemoveUser(arr: Array<MUserListDto>): void {
        //let ids = arr.map(c => c.Id);
        this.users = this.users.filter(d => !arr.includes(d));
        this.contextMenu.Close();
    }

    AddUser() {
        // this.openTemplate();
        this.sideBarOpen = true;
        this.contextMenu.Close();
    }

    UsersChanged(nUsers: Array<MUserListDto>) {
        this.users = nUsers;
        this.propagateChange(nUsers)
    }


    @ViewChild('drawerTemplate', { static: false }) drawerTemplate?: TemplateRef<{
        $implicit: { value: string };
        drawerRef: NzDrawerRef<string>;
    }>;

    // value = 'ng';

    // openTemplate(): void {
    //     const drawerRef = this.drawerService.create({
    //         nzTitle: 'Template',
    //         nzContent: this.drawerTemplate,
    //         nzContentParams: {
    //             value: this.value
    //         },
    //         nzMask: false,
    //         nzWidth: 400
    //     });

    //     drawerRef.afterOpen.subscribe(() => {
    //         console.log('Drawer(Template) open');
    //     });

    //     drawerRef.afterClose.subscribe(() => {
    //         console.log('Drawer(Template) close');
    //     });
    // }


    propagateChange(value: Array<MUserListDto>) {

        this.usersChange.emit(value);
        this.registered.forEach(fn => {
            fn(value);
        });
    }

    writeValue(value: Array<MUserListDto>): void {
        this.users = value;
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

