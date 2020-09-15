import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppInitializeService } from './app-initialize.service';
import { AppUIService, AuthenticationService } from '@services';
import { tap, share, map, filter } from 'rxjs/operators';
import { ActivatedRoute, Router, RouterLink, NavigationEnd } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {

    uiContext$ = this.uiService.UIContext$;
    sideBarCollapsed$ = this.uiService.sideBarCollapsed$;

    currentUser$ = this.auth.currentUser$;

    userName$ = this.currentUser$.pipe(
        map(user => {
            if (!user) {
                return null;
            }
            var name = user.userName;
            if(user.firstName?.trim() && user.lastName?.trim()) {
                name = `${user.firstName?.trim()} ${user.lastName?.trim()}`
            }
            
            return name;
        })
    );

    constructor(private appInitializeService: AppInitializeService, private uiService: AppUIService, private router: Router, private auth: AuthenticationService) {

        uiService.SetDefault(ui => {
            ui.Content.Scrollable = false;
            ui.Content.Container = true;
            ui.Header.Icon = null
            ui.Footer.Show = false;
        })

        // this.router.events.pipe(
        //     map(s => {
        //         if (s instanceof NavigationEnd) {
        //             return location.pathname;
        //         }
        //         return null;
        //     }),
        //     filter(p => Boolean(p))
        // ).subscribe(path => {
        //     if(path.startsWith("/identity/")){
        //         this.identity = true;
        //     }
        // });


    }

    identity = false;

    toggleSideBar() {
        this.uiService.toggleSideBar();
    }

    Login() {
        this.auth.LogIn();
    }

    Logout() {
        this.auth.LogOut();
    }

}
