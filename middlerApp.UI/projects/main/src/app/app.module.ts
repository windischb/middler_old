import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule, RoutingComponents } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { LeftMenuComponent } from './components/left-menu/left-menu.component';
import { RulesComponent } from './components/rules/rules.component';
import { MainComponent } from './components/main/main.component';
import { TreeModule } from 'angular-tree-component';
import { AngularSplitModule } from 'angular-split';
import { OverlayModule } from '@angular/cdk/overlay';
import { DoobCoreModule } from "@doob-ng/core";

@NgModule({
  declarations: [
    AppComponent,
    LeftMenuComponent,
    MainComponent,
    ...RoutingComponents
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    TreeModule.forRoot(),
    AngularSplitModule.forRoot(),
    OverlayModule,
    DoobCoreModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
