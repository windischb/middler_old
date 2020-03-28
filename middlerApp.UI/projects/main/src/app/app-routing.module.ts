import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { VariablesComponent } from './components/variables/varibales.component';


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
    component: VariablesComponent
  }
];

export const RoutingComponents = [
    HomeComponent,
    VariablesComponent
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
