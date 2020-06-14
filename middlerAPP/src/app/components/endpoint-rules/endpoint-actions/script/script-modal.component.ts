import { Component, OnInit, ElementRef } from "@angular/core";
import { FormBuilder, FormGroup } from '@angular/forms';
import { tap } from 'rxjs/operators';
import { OverlayContext } from '@doob-ng/cdk-helper';
import { EndpointAction } from '../../models/endpoint-action';
import { EndpointRulesService } from '../../endpoint-rules.service';

declare const $: any;

@Component({
    templateUrl: './script-modal.component.html',
    styleUrls: ['./script-modal.component.scss']
})
export class ScriptModalComponent implements OnInit {



    form: FormGroup;
    typings$ = this.rulesService.typings$.pipe(
        tap(t => console.log(t))
    );

    constructor(private fb: FormBuilder,
        private context: OverlayContext<EndpointAction>,
        private rulesService: EndpointRulesService) {

    }

    ngOnInit() {

        this.form = this.fb.group({
            Language: [],
            SourceCode: []
        });
        if(!this.context.data.Parameters.Language) {
            this.context.data.Parameters.Language = "Typescript"
        }
        this.form.patchValue(this.context.data.Parameters)

    }

    initRestrictionsAccordion($event: ElementRef) {
        $($event.nativeElement).accordion()
    }

    ok() {
        this.context.invoke("OK", this.form.value);
    }

    cancel() {
        this.context.handle.Close();
    }
}
