import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent
  },
  {
    path: 'endpoint-rules',
    loadChildren: () => import('./components/endpoint-rules/endpoint-rules.module').then(m => m.EndpointRulesModule)
  },
  {
    path: 'global-variables',
    loadChildren: () => import('./components/global-variables/global-variables.module').then(m => m.GlobalVariablesModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
