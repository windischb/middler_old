import { Injectable } from "@angular/core";
import { OAuthService, UserInfo, LoginOptions, AuthConfig } from 'angular-oauth2-oidc';
import { filter } from 'rxjs/operators';
import { LoggedInUser } from '../models/logged-in-user.model';
import { BehaviorSubject } from 'rxjs';
import { ColDefUtil } from '@ag-grid-community/all-modules';

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {

    private currentUserSubject$ = new BehaviorSubject<LoggedInUser>(null);
    currentUser$ = this.currentUserSubject$.asObservable();
    private authConfig: AuthConfig;

    constructor(private oauthService: OAuthService) {

        this.authConfig = {

            issuer: window.location.origin,
            redirectUri: window.location.origin,
            clientId: 'mAdmin',
            scope: 'openid roles',
            postLogoutRedirectUri: window.location.origin,
            clearHashAfterLogin: true,
            silentRefreshRedirectUri: window.location.origin + '/api/account/silentRefresh',
            showDebugInformation: true,
            sessionChecksEnabled: false,
            responseType: 'code'
        };

        // this.authConfig = {

        //     issuer: 'https://idsvr4.azurewebsites.net',
        //     redirectUri: window.location.origin,
        //     clientId: 'spa',
        //     scope: 'openid profile email api',
        //     postLogoutRedirectUri: window.location.origin,
        //     clearHashAfterLogin: true,
        //     silentRefreshRedirectUri: window.location.origin + '/api/account/silentRefresh',
        //     showDebugInformation: true,
        //     sessionChecksEnabled: false,
        //     responseType: 'code'
        // };

        this.configureWithNewConfigApi()
    }

    private configureWithNewConfigApi() {


        this.oauthService.configure(this.authConfig);
        // this.oauthService.setStorage(localStorage);
        //this.oauthService.tokenValidationHandler = new JwksValidationHandler();

       
        this.oauthService.events.subscribe(e => {
            console.debug('oauth/oidc event', e);
        });

        this.oauthService.events.pipe(
            filter(e => e.type === 'session_terminated')
        ).subscribe(e => {
            console.debug('Your session has been terminated!');
        });

        this.oauthService.events.pipe(
            filter(e => e.type === 'token_received')
        ).subscribe(e => {
            this.oauthService.loadUserProfile().then((userInfo: UserInfo) => {
                this.setUser(userInfo);
            });
        });

        this.oauthService.loadDiscoveryDocumentAndTryLogin().catch(err => { console.error(err); })
            .then(result => {

                console.log(result, this.oauthService.hasValidAccessToken())
                if (this.oauthService.hasValidAccessToken()) {
                    this.oauthService.loadUserProfile().then((userInfo: UserInfo) => {
                        this.setUser(userInfo);
                    });
                } else {
                    this.clearUser();
                }
            });

        // Optional
        this.oauthService.setupAutomaticSilentRefresh();

        
    }

    private setUser(userInfo: UserInfo) {
        const loggedInUser: LoggedInUser = {
            userName: userInfo.name,
            email: userInfo.email,
            roles: userInfo.role || [],
            firstName: userInfo.given_name,
            lastName: userInfo.family_name,
            fullName: userInfo.fullname
        }

        this.currentUserSubject$.next(loggedInUser);

    }

    private clearUser() {
        this.currentUserSubject$.next(null);
    }

    public GetAccessToken() {
        return this.oauthService.getAccessToken();
    }

    public IsAuthenticated() {
        return this.oauthService.hasValidAccessToken();
    }



    LogIn() {
        this.oauthService.initCodeFlow();
    }

    LogOut() {
        this.clearUser();
        this.oauthService.logOut();
    }
}