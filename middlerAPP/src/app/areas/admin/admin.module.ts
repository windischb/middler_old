import { NgModule } from '@angular/core';

import { AdminRoutingModule, RoutingComponents } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { IconsProviderModule } from '../../icons-provider.module';

import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { registerLocaleData, CommonModule } from '@angular/common';
import en from '@angular/common/locales/en';
import { TreeModule } from 'angular-tree-component';

import { FontAwesomeModule, FaIconLibrary, FaConfig } from '@fortawesome/angular-fontawesome';
import { fas } from '@fortawesome/free-solid-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import { GlobalImportsModule } from './global-imports.module';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { AkitaNgRouterStoreModule } from '@datorama/akita-ng-router-store';
import { environment } from '../../../environments/environment';
//import { DoobCheckBoxGroup } from "./db-checkbox-group.directive";
//import { fab } from '@fortawesome/free-brands-svg-icons';


registerLocaleData(en);

@NgModule({
  declarations: [
    AdminComponent,
    ...RoutingComponents
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    IconsProviderModule,
    HttpClientModule,
    FontAwesomeModule,
    GlobalImportsModule,
    AkitaNgRouterStoreModule
  ],
})
export class AdminUIModule {

  constructor(private library: FaIconLibrary, faConfig: FaConfig) {
    library.addIconPacks(fas, far);
  }
}
