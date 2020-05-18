import { Component, OnInit } from "@angular/core";
import { AppUIContext } from 'src/app/shared/models/app-ui-context';
import { AppUIService } from '@services';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AppSettingsService } from './app-settings.service';
import { compare } from 'fast-json-patch';
import { AppSettings } from './app-settings';

@Component({
    selector: 'app-settings',
    templateUrl: './app-settings.component.html',
    styleUrls: [ './app-settings.component.scss']
})
export class AppSettingsComponent implements OnInit {


    form: FormGroup;
    lastAppSettings: AppSettings;

    constructor(
        private uiService: AppUIService,
        private fb: FormBuilder,
        private appSettingsService: AppSettingsService) {

        uiService.Set(ui => {
            ui.Header.Title = "Application Settings",
            ui.Header.Icon = "setting",
            ui.Footer.Show = true;
            
            ui.Footer.Button1.Visible = true;
            ui.Footer.Button2.Visible = true;

            ui.Footer.Button1.OnClick = () => {
                this.appSettingsService.SetAppSettings(this.form.value);
                this.lastAppSettings = this.form.value;
                this.form.patchValue(this.lastAppSettings);
            }

            ui.Footer.Button2.Text = "Reset";
            ui.Footer.Button2.OnClick = () => {
                this.form.reset(this.lastAppSettings);
            }
            ui.Footer.Show = true;
        })

        

        this.lastAppSettings = this.appSettingsService.GetCurrentAppSettings();
    }

    ngOnInit() {

        this.form = this.fb.group({
            Debug: []
        })
        this.form.valueChanges.subscribe(v => {

            if (!this.form.valid) {
                this.uiService.Set(ui => {
                    ui.Footer.Button1.Disabled = true;
                    ui.Footer.Button1.Text = "Form not valid!"
                })
            } else {
                this.SetButtonStatus(v);
            }

        })

        this.form.patchValue(this.lastAppSettings)

        
    }

    private SetButtonStatus(rule: AppSettings) {
        var patch = compare(this.lastAppSettings, rule)

        if (patch.length > 0) {
            this.uiService.Set(ui => {
                ui.Footer.Button1.Disabled = false;
                ui.Footer.Button1.Text = "Save Changes"
                ui.Footer.Button2.Disabled = false;
            })

        } else {
            this.uiService.Set(ui => {
                ui.Footer.Button1.Disabled = true;
                ui.Footer.Button1.Text = "No Changes"
                ui.Footer.Button2.Disabled = true;
            })
        }
    }


}