import { Component, OnInit, Inject } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActionEditModalOverlayRef } from '../../modal/action-edit-modal-overlay-ref';
import { ACTION_DIALOG_DATA } from '../../modal/action-edit-modal.tokens';
import { MiddlerAction } from '../../models/middler-action';

@Component({
    templateUrl: './url-redirect-modal.component.html',
    styleUrls: ['./url-redirect-modal.component.scss']
})
export class UrlRedirectModalComponent implements OnInit {



    form: FormGroup;

    constructor(private fb: FormBuilder,
        public dialogRef: ActionEditModalOverlayRef,
        @Inject(ACTION_DIALOG_DATA) public actionContext: MiddlerAction) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            RedirectTo: [],
            Permanent: [],
            PreserveMethod: []
        });

        this.form.patchValue(this.actionContext.Parameters)

    }

    ok() {
        this.dialogRef.ok(this.form.value);
        this.dialogRef.close();
    }

    cancel() {
        this.dialogRef.close();
    }
}
