import { Component } from "@angular/core";
import { AppUIService } from '../shared/services';
import { Router } from '@angular/router';
import { AuthenticationService } from '../shared/services/authentication-service';

@Component({
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent {

    uiContext$ = this.uiService.UIContext$;
    sideBarCollapsed$ = this.uiService.sideBarCollapsed$;

    currentUser$ = this.auth.currentUser$;

    constructor(private uiService: AppUIService, private router: Router, private auth: AuthenticationService) {


        console.log("Home")
        uiService.SetDefault(ui => {
            ui.Content.Scrollable = false;
            ui.Content.Container = true;
            ui.Header.Icon = null
            ui.Footer.Show = false;
        })

        uiService.Set(ui => {
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
    
}