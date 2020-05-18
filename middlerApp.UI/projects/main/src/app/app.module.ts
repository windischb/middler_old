import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule, RoutingComponents } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { TreeModule } from 'angular-tree-component';
import { OverlayModule } from '@angular/cdk/overlay';
import { DoobCoreModule } from "@doob-ng/core";
import { DoobUIModule } from '@doob-ng/ui';
import { DoobCdkHelperModule } from '@doob-ng/cdk-helper';

@NgModule({
    declarations: [
        AppComponent,
        ...RoutingComponents
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        TreeModule.forRoot(),
        OverlayModule,
        DoobCoreModule,
        DoobUIModule,
        DoobCdkHelperModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
