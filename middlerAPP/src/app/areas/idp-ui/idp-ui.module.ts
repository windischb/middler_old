import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { IdpUIRoutingModule, RoutingComponents } from './idp-ui-routing.module';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule } from 'ng-zorro-antd/form';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
    imports: [
        CommonModule,
        IdpUIRoutingModule,
        NzFormModule,
        NzInputModule,
        NzButtonModule,
        NzCheckboxModule,
        ReactiveFormsModule
    ],
    declarations: [
        ...RoutingComponents
    ]
})
export class IdpUIModule {

}