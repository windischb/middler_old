import { Component } from '@angular/core';
import { AppInitializeService } from './app-initialize.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  title = 'main';

  constructor(private appInitializeService: AppInitializeService) {


  }
}
