import { Component, Input, TemplateRef, ViewChild, OnInit } from "@angular/core";
import { MiddlerAction } from '../../models/middler-action';



@Component({
    selector: 'action-list-item',
    templateUrl: './action-list-item.component.html',
    styleUrls: ['./action-list-item.component.scss']
})
export class ActionListItemComponent implements OnInit {


    @Input() action: MiddlerAction;

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
