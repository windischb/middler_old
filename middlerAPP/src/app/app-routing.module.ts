import { NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

const routes: Routes = [
  {
    path: 'idp',
    loadChildren: () => import('./areas/idp-ui/idp-ui.module').then(m => m.IdpUIModule)
  },
  {
    path: 'admin',
    loadChildren: () => import('./areas/admin/admin.module').then(m => m.AdminUIModule)
  },
  {
    path: '**',
    redirectTo: '/admin'
  }
];


@NgModule({
  imports: [RouterModule.forRoot(routes, { enableTracing: false, preloadingStrategy: PreloadAllModules })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
