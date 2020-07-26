import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from "@angular/core";
import { AppUIService } from '@services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { combineLatest, from, Observable, of } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { MUserDto } from '../models/m-user-dto';
import { IDPService } from '../identity.service';
import { AppUIQuery } from 'src/app/app-ui.store';

@Component({
    templateUrl: './user-details.component.html',
    styleUrls: ['./user-details.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserDetailsComponent implements OnInit {


    form: FormGroup

    private Id: string;
    public user$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.idService.GetUser(this.Id);
        }),
        mergeAll(),
        tap(user => this.setUser(user))
    )

    showDebugInformations$ = this.uiService.showDebugInformations$;

    constructor(
        private uiService: AppUIService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private idService: IDPService,
        private cref: ChangeDetectorRef
    ) {
        uiService.Set(ui => {
            ui.Header.Title = "Identity User"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#user"

            ui.Footer.Show = true;
            ui.Footer.Button1.Text = "Save";
            ui.Footer.Button2.Text = "Reset";

            ui.Footer.Button1.OnClick = () => this.Save();

            ui.Footer.Button2.OnClick = () => {
                this.form.reset(this.BaseUser);
            }
        })
    }

    ngOnInit() {

        this.form = this.fb.group({
            Id: [null],
            UserName: [null, Validators.required],
            Email: [null, Validators.required],
            FirstName: [null],
            LastName: [null],
            PhoneNumber: [null],

            EmailConfirmed: [false],
            PhoneNumberConfirmed: [false],
            TwoFactorEnabled: [false],
            LockoutEnabled: [false],
            ExpiresOn: [null],

            Claims: [[]],
            Roles: [[]]
        })

    }

    setUser(user: MUserDto) {
        if (!this.BaseUser) {
            this.BaseUser = user;
        }

        this.uiService.Set(ui => {
            ui.Header.Title = "Identity User";
            //ui.Header.SubTitle = user.UserName
            ui.Header.Icon = "form"

            ui.Footer.Button1.Visible = true;
            ui.Footer.Button1.Text = user.Id ? "Save Changes" : "Create User"
            ui.Footer.Button2.Visible = true;
        })

        this.form.patchValue(user)
    }

    onOk(date: Date) {
        date?.setHours(23, 59, 0, 0)
    }


    Save() {

        console.log(this);
        if (this.Id == 'create') {
            this.idService.CreateUser(this.form.value).subscribe();
        } else {
            this.idService.UpdateUser(this.form.value).subscribe();
        }
    }

    Reload() {
        this.idService.GetUser(this.Id).subscribe(user => {
            this.setUser(user)
            this.idService.ReLoadRoles();

        });

        
    }

    private _baseUser: MUserDto;
    private set BaseUser(rule: MUserDto) {
        this._baseUser = JSON.parse(JSON.stringify(rule));

    }
    private get BaseUser() {
        return this._baseUser;
    }
}