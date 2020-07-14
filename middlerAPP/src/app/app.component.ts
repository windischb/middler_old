import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AppInitializeService } from './app-initialize.service';
import { AppUIService } from './shared/services';
import { tap, share } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {

  uiContext$ = this.uiService.UIContext$;
  sideBarCollapsed$ = this.uiService.sideBarCollapsed$;

  constructor(private appInitializeService: AppInitializeService, private uiService: AppUIService) {

    uiService.SetDefault(ui => {
      ui.Content.Scrollable = false;
      ui.Content.Container = true;
      ui.Header.Icon = null
      ui.Footer.Show = false;
    })

  }

  toggleSideBar() {
    this.uiService.toggleSideBar();
  }
  public prepareIcon(icon: string) {
    
    if(!icon)
      return null;

    if(icon.startsWith('fa#')) {

      let res = {
        type: 'fa',
        icon: null
      }

      icon = icon.substring(3);

      if(icon.includes('|')) {
        res.icon = icon.split('|');
      } else {
        res.icon = icon;
      }
      return res;
    }

    return {
      type: 'ant',
      icon: icon
    };
  }

}
