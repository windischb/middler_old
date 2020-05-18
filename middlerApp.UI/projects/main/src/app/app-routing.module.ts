import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';


const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'rules',
    loadChildren: () => import('./components/rules/rules.module').then(m => m.RulesModule)
  },
  {
    path: 'variables',
    loadChildren: () => import('./components/variables/variables.module').then(m => m.VariablesModule)
  }
];

export const RoutingComponents = [
    HomeComponent
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
