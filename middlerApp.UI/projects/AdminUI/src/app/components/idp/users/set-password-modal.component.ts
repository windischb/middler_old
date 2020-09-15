import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OverlayContext } from '@doob-ng/cdk-helper';
import { SetPasswordDto } from '../models/set-password-dto';
import { Observable, BehaviorSubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    templateUrl: './set-password-modal.component.html',
    styleUrls: ['./set-password-modal.component.scss']
})
export class SetPasswordModalComponent implements OnInit, OnDestroy {

    form: FormGroup;

    clearPassword$ = new BehaviorSubject<boolean>(false);

    constructor(private fb: FormBuilder, private context: OverlayContext<SetPasswordDto>) {
        this.form = this.fb.group({
            Password: [null, Validators.required],
            ConfirmPassword: [null, Validators.required],
            ClearPassword: [null]
        }, { validator: this.checkPasswords });
    }

    destroy$ = new Subject();
    ngOnInit() {

        this.form.get("ClearPassword").valueChanges
            .pipe(
                takeUntil(this.destroy$)
            ).subscribe(val => {
                this.form.patchValue({
                    Password: null,
                    ConfirmPassword: null
                })
                if(val) {
                    
                    this.form.get("Password").disable();
                    this.form.get("ConfirmPassword").disable();
                } else {
                    this.form.get("Password").enable();
                    this.form.get("ConfirmPassword").enable();
                }

            })
    }

    ngOnDestroy() {
        this.destroy$.next();
    }

    ok() {

        if(this.form.value.ClearPassword) {
            this.ClearPassword();
        } else {
            this.SetPassword({
                Password: this.form.value.Password,
                ConfirmPassword: this.form.value.ConfirmPassword
            })
        }

    }

    SetPassword(model: SetPasswordDto) {
        this.context.invoke<Observable<any>>('SetPassword', model).subscribe(() => {
            this.context.handle.Close();
        });
    }

    ClearPassword() {
        this.context.invoke<Observable<any>>('ClearPassword', null).subscribe(() => {
            this.context.handle.Close();
        });
    }

    cancel() {
        this.context.handle.Close();
    }

    checkPasswords(group: FormGroup) {
        let pass = group.get('Password').value;
        let confirmPass = group.get('ConfirmPassword').value;

        return pass === confirmPass ? null : { notSame: true }
    }

}