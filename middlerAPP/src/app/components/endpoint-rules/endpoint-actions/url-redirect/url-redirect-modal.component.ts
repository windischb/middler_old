import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';

import { OverlayContext } from '@doob-ng/cdk-helper';
import { EndpointAction } from '../../models/endpoint-action';

@Component({
    templateUrl: './url-redirect-modal.component.html',
    styleUrls: ['./url-redirect-modal.component.scss']
})
export class UrlRedirectModalComponent implements OnInit {



    form: FormGroup;

    constructor(private fb: FormBuilder,
        private context: OverlayContext<EndpointAction>) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            RedirectTo: [],
            Permanent: [],
            PreserveMethod: []
        });

        this.form.patchValue(this.context.data.Parameters)

    }

    ok() {
        this.context.invoke("OK", this.form.value);
        this.context.handle.Close();
    }

    cancel() {
        this.context.handle.Close();
    }
}
