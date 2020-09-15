import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { AppUIService } from '@services';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { combineLatest, from, Observable, of } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { MUserDto } from '../models/m-user-dto';
import { IDPService } from '../idp.service';
import { IMClientDto } from '../models/m-client-dto';

@Component({
    templateUrl: './client-details.component.html',
    styleUrls: ['./client-details.component.scss']
})
export class ClientDetailsComponent implements OnInit {


    form: FormGroup


    private Id: string;
    public user$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.idService.GetClient(this.Id);
        }),
        mergeAll(),
        tap(dto => this.setClient(dto))
    )

    showDebugInformations$ = this.uiService.showDebugInformations$;

    constructor(
        private uiService: AppUIService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private idService: IDPService,
        private cref: ChangeDetectorRef
    ) {

        this.initForm();
        uiService.Set(ui => {
            ui.Header.Title = "Client"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#user"

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



    }

    initForm() {
        this.form = this.fb.group({
            Id: [null],

            ClientId: [null],
            ClientName: [null],
            Description: [null],

            Enabled: [null],
            RequireClientSecret: [null],
            RequirePkce: [null],
            AllowPlainTextPkce: [false],


            AllowedGrantTypes: [[]],

            // Consent Settings
            ClientUri: [null],
            LogoUri: [null],
            RequireConsent: [null],
            AllowRememberConsent: [false],
            ConsentLifetime: [null],

            // Claims
            Claims: [[]],
            AlwaysIncludeUserClaimsInIdToken: [false],
            UpdateAccessTokenClaimsOnRefresh: [false],
            AlwaysSendClientClaims: [false],
            ClientClaimsPrefix: [null],



            IdentityTokenLifetime: [null],
            AccessTokenLifetime: [null],
            AuthorizationCodeLifetime: [null],
            AbsoluteRefreshTokenLifetime: [null],
            SlidingRefreshTokenLifetime: [null],
            UserSsoLifetime: [null],
            DeviceCodeLifetime: [null],






            ProtocolType: [null],
            ClientSecrets: [[]],




            RequireRequestObject: [false],
            AllowAccessTokensViaBrowser: [false],
            RedirectUris: [[]],
            PostLogoutRedirectUris: [[]],
            FrontChannelLogoutUri: [null],
            FrontChannelLogoutSessionRequired: [false],
            BackChannelLogoutUri: [null],
            BackChannelLogoutSessionRequired: [false],
            AllowOfflineAccess: [false],
            AllowedScopes: [[]],

            AllowedIdentityTokenSigningAlgorithms: [null],

            RefreshTokenUsage: [null],

            RefreshTokenExpiration: [null],
            AccessTokenType: [null],
            EnableLocalLogin: [false],
            IdentityProviderRestrictions: [[]],
            IncludeJwtId: [false],


            PairWiseSubjectSalt: [null],
            AllowedCorsOrigins: [[]],

            UserCodeType: [null],

            NonEditable: [false],
        })
    }

    Save() {

        console.log(this);
        if (this.Id == 'create') {
            this.idService.CreateClient(this.form.value).subscribe();
        } else {
            this.idService.UpdateClient(this.form.value).subscribe();
        }
    }

    setClient(dto: IMClientDto) {
        if (!this.BaseDto) {
            this.BaseDto = dto;
        }

        this.uiService.Set(ui => {
            ui.Header.Title = "Client";
            //ui.Header.SubTitle = user.UserName
            ui.Header.Icon = "form"

            ui.Footer.Button1.Visible = true;
            ui.Footer.Button1.Text = dto.Id ? "Save Changes" : "Create Client"
            ui.Footer.Button2.Visible = true;
        })

        this.form.patchValue(dto)
    }

    Reload() {
        this.idService.GetClient(this.Id).subscribe(user => {
            this.setClient(user)
        });


    }

    private _baseDto: IMClientDto;
    private set BaseDto(rule: IMClientDto) {
        this._baseDto = JSON.parse(JSON.stringify(rule));

    }
    private get BaseDto() {
        return this._baseDto;
    }
}