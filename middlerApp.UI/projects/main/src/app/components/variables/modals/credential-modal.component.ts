import { Component, OnInit, HostListener } from "@angular/core";
import { OverlayContext } from "@doob-ng/cdk-helper";
import { FormGroup, FormBuilder } from '@angular/forms';
import { Variable } from "../models/variable";
import { Observable } from 'rxjs';

@Component({
    templateUrl: './credential-modal.component.html',
    styleUrls: ['./base/style.scss']
})
export class CredentialModalComponent implements OnInit {

    form: FormGroup;
    variable: Variable;
    create: boolean;

    constructor(private fb: FormBuilder, private context: OverlayContext<Variable>) {

        this.variable = context.data;
        this.create = !!this.context.metaData.create;

        this.form = this.fb.group({
            Name: [],
            Content: this.fb.group({
                UserName: [],
                Domain: [],
                Password: []
            })
        });

        this.form.patchValue(this.variable || {});
        // if(this.variable.Content) {
        //     var cont = JSON.parse(this.variable.Content || "{}")
        //     this.form.get("Content").patchValue(cont)
        // }

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

}

