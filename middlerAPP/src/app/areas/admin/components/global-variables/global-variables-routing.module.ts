import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GlobalVariablesComponent } from './global-variables.component';


const routes: Routes = [
    { path: '', component: GlobalVariablesComponent }
];

export const RoutingComponents = [
    GlobalVariablesComponent
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GlobalVariablesRoutingModule { }
