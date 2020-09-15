import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { LoginViewModel } from './models/login-view-model';
import { LoginInputModel } from './models/login-input-model';
import { LogOutModel } from './models/log-out-model';
import { Router } from '@angular/router';
import { ConsentViewModel } from './models/consent-view-model';
import { ConsentInputModel } from './models/consent-input-model';
import { ProcessConsentResult } from './models/process-content-result';

@Injectable({providedIn: 'any'})
export class IdpService {

    logOutModel: LogOutModel;

    constructor(private http: HttpClient, private router: Router) {
        
    }

    public GetLoginViewModel(returnUrl: string) {
        let params: HttpParams = null;
        if (returnUrl) {
            params = new HttpParams().set('returnUrl', returnUrl);
        }

        return this.http.get<LoginViewModel>(`/idp/account/login`, {params});
    }

    public SendLoginInputModel(loginInputModel: LoginInputModel) {
        return this.http.post<any>(`/idp/account/login`, loginInputModel);
    }

    public GetErrorModel(errorId: string) {
        return this.http.get<any>(`/idp/error/${errorId}`);
    }

    public GetLogoutViewModel(logoutId: string) {
        let params: HttpParams = null;
        if (logoutId) {
            params = new HttpParams().set('logoutId', logoutId);
        }

        return this.http.get<LogOutModel>(`/idp/account/logout`, {params})
        .subscribe(model => {

            this.logOutModel = model;
            if(!this.logOutModel.ShowLogoutPrompt){
                this.router.navigate(['/logged-out']);
            }
            if (model.Status === 'LoggedOut') {
                
            }
        });
    }

    CompleteLogOut() {
        
        return this.http.post<LogOutModel>(`/idp/account/logout`, this.logOutModel).subscribe(vm => {
            this.logOutModel = vm;
            this.router.navigate(['/logged-out']);
        });
    }

    public GetConsentViewModel(returnUrl: string) {
        let params: HttpParams = null;
        if (returnUrl) {
            params = new HttpParams().set('returnUrl', returnUrl);
        }

        return this.http.get<ConsentViewModel>(`/idp/account/consent`, {params});
    }

    public SubmitConsentOptions(model: ConsentInputModel) {
        return this.http.post<ProcessConsentResult>(`/idp/account/consent`, model);
    }
}