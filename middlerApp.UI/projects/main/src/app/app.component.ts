import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppInitializeService } from './app-initialize.service';
import { AppUIService } from './shared/services/app-ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {

    uiContext$ = this.uiService.UIContext$;

    constructor(private appInitializeService: AppInitializeService, private uiService: AppUIService) {
        uiService.SetDefault(ui => {
            ui.Content.Scrollable = false
        })
    }
}
