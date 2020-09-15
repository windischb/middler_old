import { Component } from "@angular/core";
import { AppUIService } from '@services';

@Component({
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {

  constructor(private uiService: AppUIService) {
    uiService.Set(ui => {
      ui.Header.Title = "Dashboard"
      ui.Header.Icon = "dashboard"
    })
  }

}
