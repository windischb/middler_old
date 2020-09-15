import { Component } from "@angular/core";
import { AppUIService } from '@services';

@Component({
    templateUrl: './global-variables.component.html',
    styleUrls: ['./global-variables.component.scss']
})
export class GlobalVariablesComponent {
    
    constructor(private uiService: AppUIService) {
        uiService.Set(ui => {
            ui.Header.Title = "Global Variables"
            ui.Content.Scrollable = false;
            ui.Content.Container = false;
            ui.Header.Icon = "folder"
        })

    }
}