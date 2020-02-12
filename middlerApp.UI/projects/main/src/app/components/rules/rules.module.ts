import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { RulesRoutingModule, RoutingComponents } from './rules-routing.module';
import { SpectrumModule } from '../../shared/spectrum/spectrum.module';



@NgModule({
    imports: [
        CommonModule,
        HttpClientModule,
        ReactiveFormsModule,
        RulesRoutingModule,
        SpectrumModule
    ],
    declarations: [
        ...RoutingComponents
    ],
    exports: []
})
export class RulesModule { }
