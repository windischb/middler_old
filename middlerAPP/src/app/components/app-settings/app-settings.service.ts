import { Injectable } from "@angular/core";
import { AppSettings } from './app-settings';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AppSettingsService {

    private AppSettingsSubject$ = new BehaviorSubject<AppSettings>(new AppSettings());
    public AppSettings$ = this.AppSettingsSubject$.asObservable();
    

    constructor() {

        var middlerAppSettings = localStorage.getItem("middler_appsettings");
        if(middlerAppSettings) {
            this.AppSettingsSubject$.next(JSON.parse(middlerAppSettings))
        }

    }

    public GetCurrentAppSettings() {
        return this.AppSettingsSubject$.value;
    }

    public SetAppSettings(appSettings: AppSettings) {
        this.AppSettingsSubject$.next(appSettings);
        localStorage.setItem("middler_appsettings", JSON.stringify(this.AppSettingsSubject$.value));
    }
}