import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IdpUILoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { IdpErrorComponent } from './error/error.component';
import { IdpUILogoutComponent } from './logout/logout.component';
import { IdpUILoggedOutComponent } from './logged-out/logged-out.component';
import { IdpUIConsentComponent } from './consent/consent.component';


const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },
  {
    path: 'login',
    component: IdpUILoginComponent
  },
  {
    path: 'consent',
    component: IdpUIConsentComponent
  },
  {
    path: 'logout',
    component: IdpUILogoutComponent
  },
  {
    path: 'logged-out',
    component: IdpUILoggedOutComponent
  },
  {
    path: 'error',
    component: IdpErrorComponent
  }
];

export const RoutingComponents = [
  IdpUILoginComponent,
  IdpUIConsentComponent,
  IdpErrorComponent,
  IdpUILogoutComponent,
  IdpUILoggedOutComponent
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
