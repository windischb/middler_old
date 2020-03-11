import { Component, OnInit, Inject, ElementRef } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActionEditModalOverlayRef } from '../../modal/action-edit-modal-overlay-ref';
import { ACTION_DIALOG_DATA } from '../../modal/action-edit-modal.tokens';
import { MiddlerAction } from '../../models/middler-action';

declare const $: any;

@Component({
    templateUrl: './script-modal.component.html',
    styleUrls: ['./script-modal.component.scss']
})
export class ScriptModalComponent implements OnInit {



    form: FormGroup;

    constructor(private fb: FormBuilder,
        public dialogRef: ActionEditModalOverlayRef,
        @Inject(ACTION_DIALOG_DATA) public actionContext: MiddlerAction) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            Language: [],
            SourceCode: []
        });

        this.form.patchValue(this.actionContext.Parameters)

    }

    initRestrictionsAccordion($event: ElementRef) {
        $($event.nativeElement).accordion()
    }

    ok() {
        this.dialogRef.ok(this.form.value);
        this.dialogRef.close();
    }

    cancel() {
        this.dialogRef.close();
    }
}
