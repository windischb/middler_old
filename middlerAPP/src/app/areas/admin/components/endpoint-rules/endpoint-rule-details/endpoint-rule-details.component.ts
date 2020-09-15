import { Component } from "@angular/core";
import { AppUIService } from '@services';
import { ActivatedRoute } from '@angular/router';
import { EndpointRulesService } from '../endpoint-rules.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { combineLatest, BehaviorSubject } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { EndpointRule } from '../models/endpoint-rule';
import { compare } from 'fast-json-patch';
import { NzTabChangeEvent } from 'ng-zorro-antd/tabs/public-api';
import { EndpointAction } from '../models/endpoint-action';

@Component({
    selector: 'endpoint-rule-details',
    templateUrl: './endpoint-rule-details.component.html',
    styleUrls: ['./endpoint-rule-details.component.scss']
})
export class EndpointRuleDetailsComponent {

    private Id: string;

    public ruleActions: Array<EndpointAction>;

    public rule$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.rulesService.GetRule(this.Id);
        }),
        mergeAll(),
        tap(rule => {
            if (!this.BaseRule) {
                this.BaseRule = rule;
            }
            
            this.rulesService.GetActionsForRule(rule.Id).subscribe(actions => this.ruleActions = actions);

            this.uiService.Set(ui => {
                ui.Header.Title = "Endpoint Rule";
                ui.Header.SubTitle = rule.Name
                ui.Header.Icon = "form"

                ui.Footer.Button1.Visible = true;
                ui.Footer.Button2.Visible = true;
            })

            this.form.patchValue(rule)
        })
    )


    showDebugInformations$ = this.uiService.showDebugInformations$;
    form: FormGroup;

    private selectedTab = new BehaviorSubject<number>(0);

    constructor(
        private uiService: AppUIService,
        private route: ActivatedRoute,
        private rulesService: EndpointRulesService,
        private fb: FormBuilder) {

        this.uiService.Set(ui => {
            ui.Footer.Button1.Disabled = true;
            ui.Footer.Button2.Disabled = true;
            ui.Footer.Button1.OnClick = () => {
                this.save()
            }
            ui.Footer.Button2.Text = "Reset";
            ui.Footer.Button2.OnClick = () => {
                this.form.reset(this.BaseRule);
            }
            ui.Footer.Show = true;
        })


        this.form = this.fb.group({
            Id: [null],
            Name: [null, Validators.required],
            Scheme: [],
            Hostname: [],
            Path: [],
            //Actions: [[]], //this.fb.array([]),
            HttpMethods: [[]],
            //Permissions: [[]],
            Order: [],
            Enabled: [false]
        })



    }

    ngOnInit() {

        this.selectedTab.subscribe(index => {
            if (index === 0) {
                this.uiService.Set(ui => {
                    ui.Footer.Button1.Visible = true;
                    ui.Footer.Button2.Visible = true;
                })
            } else {
                this.uiService.Set(ui => {
                    ui.Footer.Button1.Visible = false;
                    ui.Footer.Button2.Visible = false;
                })
            }
        })

        this.form.get('Name').valueChanges.subscribe(name => this.uiService.Set(ui => ui.Header.SubTitle = name))
        this.form.valueChanges.subscribe(v => {

            if (!this.form.valid) {
                this.uiService.Set(ui => {
                    ui.Footer.Button1.Disabled = true;
                    ui.Footer.Button1.Text = "Form not valid!"
                })
            } else {
                this.SetButtonStatus(v);
            }

        })
    }

    GeneratePatchDocument(base?: any, comp?: any) {
        base = base ?? this.BaseRule;
        comp = comp ?? this.form.value;

        base.Actions = null;
        base.Permissions = null;
        comp.Actions = null;
        comp.Permissions = null;
        var patchDocument = compare(base, comp);
        return patchDocument;
    }



    save() {
        this.rulesService.UpdatePartial(this.Id, this.GeneratePatchDocument()).pipe(
            tap(() => {
                this.BaseRule = this.form.value
                this.SetButtonStatus(this.BaseRule)
            })
        ).subscribe()
    }

    private SetButtonStatus(rule: EndpointRule) {
        var patch = this.GeneratePatchDocument(this.BaseRule, rule)

        if (patch.length > 0) {
            this.uiService.Set(ui => {
                ui.Footer.Button1.Disabled = false;
                ui.Footer.Button1.Text = "Save Changes"


                ui.Footer.Button2.Disabled = false;

            })

        } else {
            this.uiService.Set(ui => {
                ui.Footer.Button1.Disabled = true;
                ui.Footer.Button1.Text = "No Changes"

                ui.Footer.Button2.Disabled = true;
            })
        }
    }

    tabChanged(event: NzTabChangeEvent) {
        this.selectedTab.next(event.index ?? 0);
    }

    isNotSelected(value: string): boolean {
        return this.form.value.HttpMethods.indexOf(value) === -1;
    }

    private _baseRule: EndpointRule;
    private set BaseRule(rule: EndpointRule) {
        this._baseRule = JSON.parse(JSON.stringify(rule));

    }
    private get BaseRule() {
        return this._baseRule;
    }

}