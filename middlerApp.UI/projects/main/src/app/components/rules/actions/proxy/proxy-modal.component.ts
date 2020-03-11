import { Component, OnInit, Inject } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActionEditModalOverlayRef } from '../../modal/action-edit-modal-overlay-ref';
import { ACTION_DIALOG_DATA } from '../../modal/action-edit-modal.tokens';
import { MiddlerAction } from '../../models/middler-action';

@Component({
    templateUrl: './proxy-modal.component.html',
    styleUrls: ['./proxy-modal.component.scss']
})
export class PRoxyModalComponent implements OnInit {



    form: FormGroup;

    constructor(private fb: FormBuilder,
        public dialogRef: ActionEditModalOverlayRef,
        @Inject(ACTION_DIALOG_DATA) public actionContext: MiddlerAction) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            DestinationUrl: [],
            UserIntermediateStream: [],
            AddXForwardedHeaders: [],
            CopyXForwardedHeaders: []
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
