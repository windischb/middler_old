import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Variable } from '../../models/variable';
import { FormGroup } from '@angular/forms';

@Component({
    selector: 'basic-modal',
    templateUrl: './basic-modal.component.html',
    styleUrls: ['../style.scss'],
    host: {
        class: 'flex-grow-column'
    }
})
export class BasicModalComponent {

    @Input() variable: Variable;
    @Output() ok = new EventEmitter();
    @Output() cancel = new EventEmitter();

    @Input() form: FormGroup;

}
