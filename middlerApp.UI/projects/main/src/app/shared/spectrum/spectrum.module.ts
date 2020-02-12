import { NgModule } from "@angular/core";
import { CommonModule } from '@angular/common';
import { SpectrumTabsComponent, SpectrumTabComponent } from './tabs';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        SpectrumTabsComponent,
        SpectrumTabComponent,
    ],
    exports: [
        SpectrumTabsComponent,
        SpectrumTabComponent
    ]
})
export class SpectrumModule {

}
