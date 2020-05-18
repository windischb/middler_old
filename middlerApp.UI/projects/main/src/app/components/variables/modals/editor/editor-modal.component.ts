import { Component } from "@angular/core";
import { FormGroup, FormBuilder } from '@angular/forms';
import { Variable } from '../../models/variable';
import { OverlayContext } from '@doob-ng/cdk-helper';
import { LanguageHelper } from '@doob-ng/editor';
import { Observable } from 'rxjs';

@Component({
    templateUrl: './editor-modal.component.html',
    styleUrls: ['../style.scss']
})
export class EditorModalComponent {

    form: FormGroup;
    variable: Variable;
    create: boolean;
    language: string;
    raw: boolean;

    constructor(private fb: FormBuilder, private context: OverlayContext<Variable>) {

        this.variable = context.data;
        this.create = !!this.context.metaData.create;
        this.raw = !!this.context.metaData.raw;

        this.form = this.fb.group({
            Name: [],
            Content: [],
            Extension: [],
            Language: [{value: null, disabled: !(this.create || this.raw)}]
        });

        if (this.variable.Content === null || this.variable.Content === undefined) {
            this.variable.Content = ""
        } else if (typeof this.variable.Content === 'object') {
            this.variable.Content = JSON.stringify(this.variable.Content)
        } else if (this.variable.Content instanceof Array) {
            this.variable.Content = JSON.stringify(this.variable.Content)
        } else if(typeof this.variable.Content !== 'string') {
            this.variable.Content = `${this.variable.Content}`
        }
        this.form.patchValue(this.variable);
        this.form.get("Language").patchValue(this.getLanguage(this.variable.Extension))
    }

    ngOnInit() {

    }

    ok() {
        this.context.invoke<Observable<any>>('ok', this.form.value).subscribe(() => {
            this.context.handle.Close();
        });
    }

    cancel() {
        this.context.handle.Close();
    }

    getLanguage(extension: string) {
        if(!extension){
            return 'plaintext';
        }
        if(extension.startsWith('.')){
            return LanguageHelper.GetLanguageFromFileExtension(extension) || 'plaintext';
        } else {
            return extension.toLowerCase();
        }

    }
}
