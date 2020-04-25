import { Component, OnInit, HostListener } from "@angular/core";
import { OverlayContext } from "@doob-ng/cdk-helper";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Variable } from "../../models/variable";
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    templateUrl: './string-modal.component.html',
    styleUrls: ['../style.scss']
})
export class StringModalComponent implements OnInit {

    form: FormGroup;
    variable: Variable;
    create: boolean;
    valid: boolean;
    error:string;
    private destroy$ = new Subject();

    constructor(private fb: FormBuilder, private context: OverlayContext<Variable>) {

        this.variable = context.data;
        this.create = !!this.context.metaData.create;

        this.form = this.fb.group({
            Name: [null, Validators.required],
            Content: []
        });

        // this.form.statusChanges.subscribe(valid => {
        //     console.log(valid);
        //     this.valid = valid === 'VALID';
        // })
        this.form.patchValue(this.variable || {});

        this.form.get('Name').valueChanges.pipe(
            takeUntil(this.destroy$)
        ).subscribe(value => {
            this.context.invoke('changed', null);
        })


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

