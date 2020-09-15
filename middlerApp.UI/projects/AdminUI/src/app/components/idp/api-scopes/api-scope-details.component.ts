import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { AppUIService } from '@services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { combineLatest, from, Observable, of } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { IDPService } from '../idp.service';
import { IMScopeDto } from '../models/m-scope-dto';


@Component({
    templateUrl: './api-scope-details.component.html',
    styleUrls: ['./api-scope-details.component.scss']
})
export class ApiScopeDetailsComponent implements OnInit {


    form: FormGroup


    private Id: string;
    public user$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.idService.GetApiScope(this.Id);
        }),
        mergeAll(),
        tap(dto => this.setApiScope(dto))
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
            ui.Header.Title = "ApiScope"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#cube"

            ui.Footer.Show = true;
            ui.Footer.Button1.Text = "Save";
            ui.Footer.Button2.Text = "Reset";

            ui.Footer.Button1.OnClick = () => this.Save();

            ui.Footer.Button2.OnClick = () => {
                this.form.reset(this.BaseDto);
            }
        })
    }

    ngOnInit() {

        this.form = this.fb.group({
            Id: [null],
            Enabled: [],
            Name: [null, Validators.required],
            DisplayName: [null, Validators.required],
            Description: [null],
            Required: [],
            Emphasize: [],
            ShowInDiscoveryDocument: [],
            UserClaims: [[]],
            Properties: []
        })

    }


    onOk(date: Date) {
        date.setHours(23, 59, 0, 0)
    }


    Save() {

        console.log(this);
        if (this.Id == 'create') {
            this.idService.CreateApiScope(this.form.value).subscribe();
        } else {
            this.idService.UpdateApiScope(this.form.value).subscribe();
        }
    }

    setApiScope(dto: IMScopeDto) {
        if (!this.BaseDto) {
            this.BaseDto = dto;
        }

        this.uiService.Set(ui => {
            ui.Header.Title = "ApiScope";
            //ui.Header.SubTitle = user.UserName
            ui.Header.Icon = "form"

            ui.Footer.Button1.Visible = true;
            ui.Footer.Button1.Text = dto.Id ? "Save Changes" : "Create ApiScope"
            ui.Footer.Button2.Visible = true;
        })

        this.form.patchValue(dto)
    }

    Reload() {
        this.idService.GetApiScope(this.Id).subscribe(apiresource => {
            this.setApiScope(apiresource)
            this.idService.ReLoadUsers();
        });


    }

    private _baseDto: IMScopeDto;
    private set BaseDto(rule: IMScopeDto) {
        this._baseDto = JSON.parse(JSON.stringify(rule));

    }
    private get BaseDto() {
        return this._baseDto;
    }
}