import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { MiddlerAction } from '../../models/middler-action';
import { OverlayContext } from '@doob-ng/cdk-helper';

@Component({
    templateUrl: './proxy-modal.component.html',
    styleUrls: ['./proxy-modal.component.scss']
})
export class ProxyModalComponent implements OnInit {



    form: FormGroup;

    constructor(private fb: FormBuilder,
        private context: OverlayContext<MiddlerAction>) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            DestinationUrl: [],
            UserIntermediateStream: [],
            AddXForwardedHeaders: [],
            CopyXForwardedHeaders: []
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
