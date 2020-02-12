import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RulesComponent } from './rules.component';
import { RuleDetailsComponent } from './rule-details.component';


const routes: Routes = [
    { path: '', component: RulesComponent },
    { path: ':id', component: RuleDetailsComponent}
];

export const RoutingComponents = [
    RulesComponent,
    RuleDetailsComponent
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RulesRoutingModule { }
