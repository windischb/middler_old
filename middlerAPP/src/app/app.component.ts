import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppInitializeService } from './app-initialize.service';
import { AppUIService } from './shared/services';
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



    constructor(private appInitializeService: AppInitializeService, private uiService: AppUIService, private router: Router) {

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


}
