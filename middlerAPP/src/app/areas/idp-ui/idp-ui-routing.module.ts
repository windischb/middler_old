import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IdpUILoginComponent } from './login/login.component';


const routes: Routes = [
  {
    path: 'login',
    component: IdpUILoginComponent
  }
];

export const RoutingComponents = [
  IdpUILoginComponent
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IdpUIRoutingModule { }
