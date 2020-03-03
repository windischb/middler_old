import { Component, ChangeDetectionStrategy } from "@angular/core";
import { UIService } from './ui.service';

@Component({
    selector: 'main',
    templateUrl: './main.component.html',
    styleUrls: ['./main.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MainComponent {


    uiContext$ = this.uiService.UIContext$;

    constructor(private uiService: UIService) {

    }
}
