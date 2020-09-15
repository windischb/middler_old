import { Component, ChangeDetectionStrategy, ChangeDetectorRef } from "@angular/core";
import { IdpService } from '../shared/services/idp/idp.service';
import { ActivatedRoute } from '@angular/router';
import { ConsentViewModel } from '../shared/services/idp/models/consent-view-model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ConsentInputModel } from '../shared/services/idp/models/consent-input-model';

@Component({
    templateUrl: './consent.component.html',
    styleUrls: ['./consent.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class IdpUIConsentComponent {

    viewModel: ConsentViewModel;
    form: FormGroup;

    constructor(private idpService: IdpService, private route: ActivatedRoute, private fb: FormBuilder, private cref: ChangeDetectorRef) { }

    ngOnInit() {

        const par = this.route.queryParamMap.subscribe(qmap => {

            let returnUrl: string = null;
            for (const key of qmap.keys) {
                if (key.toLowerCase() === 'returnurl') {
                    returnUrl = qmap.get(key);
                }
            }

            this.idpService.GetConsentViewModel(returnUrl).subscribe(vm => {

                this.viewModel = vm;

                this.createForm(vm);
                this.cref.detectChanges();

            });
        });


    }

    createForm(vm: ConsentViewModel) {

        this.form = this.fb.group({
            'RememberConsent': [false],
            'ScopesConsented': [[]]
        });

        var identScopes = this.viewModel.IdentityScopes?.filter(s => s.Checked).map(s => s.Value) ?? [];
        var resScopes = this.viewModel.ApiScopes?.filter(s => s.Checked).map(s => s.Value) ?? [];

        var checkedScopes = [...identScopes, ...resScopes];
        this.form.patchValue({
            ScopesConsented: checkedScopes,
            RememberConsent: vm.RememberConsent
        })

    }

    toggleRemember() {
        const rem = this.form.get("RememberConsent");
        rem.patchValue(!rem.value)
    }

    Submit(button: string) {

        const cmodel = new ConsentInputModel();
        cmodel.RememberConsent = this.form.value.RememberConsent;
        cmodel.ReturnUrl = this.viewModel.ReturnUrl;
        cmodel.ScopesConsented = this.form.value.ScopesConsented;
        cmodel.Button = button;

        console.log(this.form.value, cmodel);
        this.idpService.SubmitConsentOptions(cmodel).subscribe(okResp => {
            if (okResp.IsRedirect) {
                window.location.href = okResp.RedirectUri;
            }
        }, errorResp => {
            console.error(errorResp);
        });
    }

}