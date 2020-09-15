import { Component } from "@angular/core";
import { AppUIService } from '@services';

@Component({
    templateUrl: './endpoint-rules.component.html',
    styleUrls: ['./endpoint-rules.component.scss']
})
export class EndpointRulesComponent {
    
    constructor(private uiService: AppUIService) {
        uiService.Set(ui => {
            ui.Header.Title = "Endpoint Rules"
            ui.Content.Scrollable = false;
            ui.Header.Icon = "fa#stream"
        })
    }
}