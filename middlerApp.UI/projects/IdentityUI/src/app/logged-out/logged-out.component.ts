import { Component, ChangeDetectionStrategy } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IdpService } from '../shared/services/idp/idp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { LoginInputModel } from '../shared/services/idp/models/login-input-model';
import { AuthenticationService } from '../shared/services/authentication-service';
import { Location } from '@angular/common';
import { LogOutModel } from '../shared/services/idp/models/log-out-model';

@Component({
    templateUrl: './logged-out.component.html',
    styleUrls: ['./logged-out.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class IdpUILoggedOutComponent {

    logOutModel: LogOutModel = null;
   

    constructor(private idpService: IdpService, private route: ActivatedRoute, private auth: AuthenticationService, private location: Location) { }

    ngOnInit() {

        console.log(this.idpService.logOutModel);
        
        if (!this.idpService.logOutModel) {
            this.location.back();
            return;
        }

        if (!this.idpService.logOutModel.ClientName) {
            window.location.href = '/';
        }
        if (this.idpService.logOutModel.AutomaticRedirectAfterSignOut) {
            window.location.href = this.idpService.logOutModel.PostLogoutRedirectUri;
            return;
        }

        this.logOutModel = this.idpService.logOutModel;
    }

}