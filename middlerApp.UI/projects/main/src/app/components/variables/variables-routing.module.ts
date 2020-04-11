import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VariablesExplorerComponent } from './components/variables-explorer';



const routes: Routes = [
    {
        path: '',
        component: VariablesExplorerComponent
    }
];

export const RoutingComponents = [
    VariablesExplorerComponent
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class VariablesRoutingModule { }
