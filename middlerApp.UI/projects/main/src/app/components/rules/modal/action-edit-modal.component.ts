import { Component, Inject } from "@angular/core";
import { ActionEditModalOverlayRef } from './action-edit-modal-overlay-ref';
import { ACTION_DIALOG_DATA } from './action-edit-modal.tokens';

@Component({
    templateUrl: './action-edit-modal.component.html',
    styleUrls: ['./action-edit-modal.component.scss']
})
export class ActionEditModalComponent {

    constructor(
        public dialogRef: ActionEditModalOverlayRef,
        @Inject(ACTION_DIALOG_DATA) public actionContext: any) { }

}
