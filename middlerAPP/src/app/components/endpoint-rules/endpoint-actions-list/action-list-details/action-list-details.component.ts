import { Component, Input, TemplateRef, ViewChild, OnInit } from "@angular/core";
import { EndpointAction } from '../../models/endpoint-action';


@Component({
    selector: 'action-list-details',
    templateUrl: './action-list-details.component.html',
    styleUrls: ['./action-list-details.component.scss']
})
export class ActionListDetailsComponent implements OnInit {


    @Input() action: EndpointAction;

    @ViewChild('urlredirect', {static: true}) UrlRedirect: TemplateRef<any>;
    @ViewChild('urlrewrite', {static: true}) UrlRewrite: TemplateRef<any>;
    @ViewChild('script', {static: true}) Script: TemplateRef<any>;

    template: TemplateRef<any>;

    ngOnInit() {

        this.template = this.determineTemplate();
    }

    determineTemplate() {

        switch (this.action.ActionType) {
            case 'UrlRedirect': {
                return this.UrlRedirect;
            }
            case 'UrlRewrite': {
                return this.UrlRewrite;
            }
            case 'Script': {
                return this.Script;
            }
        }
    }

}
