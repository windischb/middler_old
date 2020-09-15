import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { HomeRoutingModule, RoutingComponents } from './home-routing.module';
import { GlobalImportsModule } from '../global-imports.module';

@NgModule({
    imports: [
        CommonModule,
        HomeRoutingModule,
        GlobalImportsModule
    ],
    declarations: [
        ...RoutingComponents
    ]
})
export class HomeModule {

}