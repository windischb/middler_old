import { Component, OnInit, ElementRef, ViewChild, TemplateRef, ViewContainerRef, ChangeDetectionStrategy, ContentChild } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { RulesService } from './rules.service';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { Observable, Subject, Subscription, fromEvent, of, combineLatest } from 'rxjs';
import { map, mergeAll, tap, filter, take } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { OverlayRef, Overlay } from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';
import { compare } from 'fast-json-patch';
import { ActionsListComponent } from './actions-list.component';
import { UIService } from '../main/ui.service';

declare var $: any;

@Component({
    templateUrl: './rule-details.component.html',
    styleUrls: ['./rule-details.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RuleDetailsComponent implements OnInit {


    //public ruleSubject$: Subject<MiddlerRuleDto> = new Subject<MiddlerRuleDto>();
    private originalRule: MiddlerRuleDto;

    private Id: string;
    private Index: string;
    private Order: number;

    public rule$ = combineLatest(this.route.queryParamMap, this.route.paramMap).pipe(
        map(([queryParamMap, paramMap]) => {
            this.Id = paramMap.get('id')

            this.Index = queryParamMap.get('index')

            if (this.Id === 'create') {

                if (!this.rulesService.MiddlerRules || this.rulesService.MiddlerRules.length == 0) {

                    return this.rulesService.GetAll()
                        .pipe(
                            map(res => of(new MiddlerRuleDto())),
                            mergeAll()
                        )

                }
                return of(new MiddlerRuleDto())
            }


            return this.rulesService.Get(this.Id);
        }),
        mergeAll(),
        tap(rule => {
            if (!this.originalRule) {
                this.originalRule = JSON.parse(JSON.stringify(rule));
            }

            if (this.Id === 'create') {
                rule.Order = this.calculateNewOrder();
                this.uiService.HeaderTitle = "Create Rule";
                this.uiService.HeaderIcon = "add"
            } else {
                this.uiService.HeaderTitle = "Edit Rule";
                this.uiService.HeaderSubTitle = rule.Name
                this.uiService.HeaderIcon = "edit"
            }




            this.form.patchValue(rule)
        })
    )



    form: FormGroup;

    // get ActionsArr() {
    //     let acts = this.form.get("Actions") as FormArray;
    //     return acts;
    // }

    @ViewChild(ActionsListComponent) actionsList: ActionsListComponent;

    constructor( private uiService: UIService, private route: ActivatedRoute, private rulesService: RulesService, private fb: FormBuilder, public overlay: Overlay, public viewContainerRef: ViewContainerRef) {


        this.form = this.fb.group({
            Id: [null],
            Name: [null, Validators.required],
            Scheme: [],
            Hostname: [],
            Path: [],
            Actions: [[]], //this.fb.array([]),
            HttpMethods: [[]],
            Permissions: [[]],
            Order: [],
            Enabled: [false]
        })


    }

    private calculateNewOrder() {

        if (!this.Index) {
            this.Index = 'last'
        }
        if (!this.rulesService.MiddlerRules) {
            return 10
        } else {

            const rulesLength = this.rulesService.MiddlerRules.length;

            if (this.Index === 'first') {

                let f = this.rulesService.MiddlerRules[0].Order
                return f / 2

            } else if (this.Index === 'last') {

                let f = this.rulesService.MiddlerRules[rulesLength - 1].Order
                return f + 10

            } else {

                const ind = parseInt(this.Index)

                if(ind <= (rulesLength -1)) {
                    let before = this.rulesService.MiddlerRules[ind -1].Order;
                    let after = this.rulesService.MiddlerRules[ind].Order;
                    return ((after - before) / 2) + before
                } else {
                    let f = this.rulesService.MiddlerRules[rulesLength - 1].Order
                    return f + 10
                }

            }

        }

    }

    // private buildActionFormgroup() {
    //     const fg = this.fb.group({
    //         ActionType: [null, Validators.required],
    //         ContinueAfterwards: [false],
    //         WriteStreamDirect: [false],
    //         Parameters: this.fb.group({})
    //     })
    //     fg.get('ActionType').valueChanges.subscribe(val => {
    //         switch (val) {
    //             case 'UrlRedirect': {
    //                 fg.setControl('Parameters', this.buildUrlRedirectFormGroup())
    //             }
    //         }
    //     })

    //     return fg;
    // }

    // private buildUrlRedirectFormGroup() {
    //     const fg = this.fb.group({
    //         RedirectTo: [null, Validators.required],
    //         Permanent: [false],
    //         PreserveMethod: [false]
    //     })
    //     return fg;
    // }

    ngOnInit() {

        this.form.get('Name').valueChanges.subscribe(name => this.uiService.HeaderSubTitle = name)

    }

    onrightClick($event: MouseEvent) {
        $event.preventDefault();
        console.log($event)
    }

    initRestrictionsAccordion($event: ElementRef) {
        $($event.nativeElement).accordion()
    }

    GeneratePatchDocument() {
        var patchDocument = compare(this.originalRule, this.form.value);
        return patchDocument;
    }

    save() {
        if (this.Id === 'create') {
            this.rulesService.Add(this.form.value).subscribe()
        } else {
            this.rulesService.UpdatePartial(this.Id, this.GeneratePatchDocument())
        }
    }

    AddAction() {

        this.actionsList.AddAction();

    }

}
