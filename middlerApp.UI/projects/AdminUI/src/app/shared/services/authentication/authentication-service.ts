import { Injectable } from "@angular/core";
import { OAuthService, UserInfo, LoginOptions, AuthConfig } from 'angular-oauth2-oidc';
import { filter } from 'rxjs/operators';
import { LoggedInUser } from './logged-in-user.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {

    private currentUserSubject$ = new BehaviorSubject<LoggedInUser>(null);
    currentUser$ = this.currentUserSubject$.asObservable();
    private authConfig: AuthConfig;

    constructor(private oauthService: OAuthService) {

        this.authConfig = {

            issuer: 'https://localhost:4300',
            redirectUri: window.location.origin,
            clientId: 'mAdmin',
            scope: 'openid roles offline_access IdentityServerApi',
            postLogoutRedirectUri: window.location.origin,
            clearHashAfterLogin: true,
            silentRefreshRedirectUri: window.location.origin + '/api/account/silentRefresh',
            showDebugInformation: true,
            sessionChecksEnabled: false,
            responseType: 'code'
        };

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

        const loggedInUser = new LoggedInUser();
        if (userInfo != null) {
            loggedInUser.userName = userInfo.name;
            loggedInUser.email = userInfo.email;
            loggedInUser.roles = userInfo.role || [];
            loggedInUser.firstName = userInfo.given_name;
            loggedInUser.lastName = userInfo.family_name;
            loggedInUser.fullName = userInfo.fullname;
        }


        this.currentUserSubject$.next(loggedInUser);

    }

    private clearUser() {
        this.currentUserSubject$.next(new LoggedInUser());
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