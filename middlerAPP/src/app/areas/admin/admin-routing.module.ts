import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { AppSettingsComponent } from './components/app-settings/app-settings.component';
import { AdminComponent } from './admin.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      {
        path: '',
        component: DashboardComponent
      },
      {
        path: 'app-settings',
        component: AppSettingsComponent
      },
      {
        path: 'endpoint-rules',
        loadChildren: () => import('./components/endpoint-rules/endpoint-rules.module').then(m => m.EndpointRulesModule)
      },
      {
        path: 'global-variables',
        loadChildren: () => import('./components/global-variables/global-variables.module').then(m => m.GlobalVariablesModule)
      },
      {
        path: 'idp',
        loadChildren: () => import('./components/idp/idp.module').then(m => m.IDPModule)
      }
    ]
  }
  
];

export const RoutingComponents = [
  DashboardComponent,
  AppSettingsComponent
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
