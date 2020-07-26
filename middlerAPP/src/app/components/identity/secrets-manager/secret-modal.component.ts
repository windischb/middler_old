import { Component, Input, Output, EventEmitter } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Secret } from '../models/secret';
import { OverlayContext } from '@doob-ng/cdk-helper';
import { takeUntil } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
    selector: 'secret-modal',
    templateUrl: './secret-modal.component.html',
    styleUrls: ['./secret-modal.component.scss'],
    host: {
        class: 'flex-grow-column'
    }
})
export class SecretModalComponent {

    secret: Secret;
    form: FormGroup;

    constructor(private fb: FormBuilder, private context: OverlayContext<Secret>) {

        this.secret = context.data;


        this.form = this.fb.group({
            Type: [null, Validators.required],
            Value: [],
            Description: [],
            Expiration: []
        });

        // this.form.statusChanges.subscribe(valid => {
        //     console.log(valid);
        //     this.valid = valid === 'VALID';
        // })
        this.form.patchValue(this.secret || {});

    }

    onOk(date: Date) {
        date?.setHours(23, 59, 0, 0)
    }

    ok() {

        this.context.invoke<Observable<any>>('ok', this.form.value).subscribe(() => {
            this.context.handle.Close();
        });
    }

    cancel() {
        this.context.handle.Close();
    }

    onValueChange(value: Date): void {
        console.log(`Current value: ${value}`);
    }

    onPanelChange(change: { date: Date; mode: string }): void {
        console.log(`Current value: ${change.date}`);
        console.log(`Current mode: ${change.mode}`);
    }
}
