import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { AppUIService } from '@services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { combineLatest, from, Observable, of } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { IDPService } from '../idp.service';
import { IMApiResourceDto } from '../models/m-api-resource-dto';


@Component({
    templateUrl: './api-resource-details.component.html',
    styleUrls: ['./api-resource-details.component.scss']
})
export class ApiResourceDetailsComponent implements OnInit {


    form: FormGroup


    private Id: string;
    public user$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.idService.GetApiResource(this.Id);
        }),
        mergeAll(),
        tap(dto => this.setApiResource(dto))
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
            ui.Header.Title = "ApiResource"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#cubes"

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

            Name: [null, Validators.required],
            DisplayName: [null, Validators.required],
            Description: [null],


            Enabled: [],
            AllowedAccessTokenSigningAlgorithms: [],
            ShowInDiscoveryDocument: [],
            Secrets: [[]],
            Scopes: [[]],
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
            this.idService.CreateApiResource(this.form.value).subscribe();
        } else {
            this.idService.UpdateApiResource(this.form.value).subscribe();
        }
    }

    setApiResource(dto: IMApiResourceDto) {
        if (!this.BaseDto) {
            this.BaseDto = dto;
        }

        this.uiService.Set(ui => {
            ui.Header.Title = "ApiResource";
            //ui.Header.SubTitle = user.UserName
            ui.Header.Icon = "form"

            ui.Footer.Button1.Visible = true;
            ui.Footer.Button1.Text = dto.Id ? "Save Changes" : "Create ApiResource"
            ui.Footer.Button2.Visible = true;
        })

        this.form.patchValue(dto)
    }

    Reload() {
        this.idService.GetApiResource(this.Id).subscribe(apiresource => {
            this.setApiResource(apiresource)
            this.idService.ReLoadUsers();
        });


    }

    private _baseDto: IMApiResourceDto;
    private set BaseDto(rule: IMApiResourceDto) {
        this._baseDto = JSON.parse(JSON.stringify(rule));

    }
    private get BaseDto() {
        return this._baseDto;
    }
}