import { Component, ChangeDetectionStrategy } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IdpService } from '../shared/services/idp/idp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { LoginInputModel } from '../shared/services/idp/models/login-input-model';
import { AuthenticationService } from '../shared/services/authentication-service';
import { Location } from '@angular/common';

@Component({
    templateUrl: './logout.component.html',
    styleUrls: ['./logout.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class IdpUILogoutComponent {

    viewModel = null;
    errors: any;


    constructor(private idpService: IdpService, private route: ActivatedRoute, private auth: AuthenticationService, private location: Location) { }

    ngOnInit(): void {
        this.route.queryParamMap.pipe(take(1)).subscribe(qmap => {

            let logoutid: string = null;
            for (const key of qmap.keys) {
                if (key.toLowerCase() === 'logoutid') {
                    logoutid = qmap.get(key);
                }
            }

            this.idpService.GetLogoutViewModel(logoutid);
        });
    }

    public Cancel() {
        this.location.back();
    }

    public Logout() {
        this.idpService.CompleteLogOut();
    }

}