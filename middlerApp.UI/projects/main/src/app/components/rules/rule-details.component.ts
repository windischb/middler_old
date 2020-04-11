import { Component, OnInit, ElementRef, ViewChild, ViewContainerRef, ChangeDetectionStrategy, TemplateRef, ViewChildren, QueryList } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { RulesService } from './rules.service';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { combineLatest } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Overlay } from '@angular/cdk/overlay';
import { compare } from 'fast-json-patch';
import { ActionsListComponent } from './actions-list.component';
import { AppUIService } from '../../shared/services/app-ui.service';


declare var $: any;

@Component({
    templateUrl: './rule-details.component.html',
    styleUrls: ['./rule-details.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RuleDetailsComponent implements OnInit {




    private Id: string;
    //private Index: string;
    //private Order: number;

    public rule$ = combineLatest(this.route.paramMap, this.route.queryParamMap, this.route.fragment).pipe(
        map(([paramMap, queryParamMap, fragment]) => {
            this.Id = paramMap.get('id')
            return this.rulesService.Get(this.Id);
        }),
        mergeAll(),
        tap(rule => {
            if(!this.BaseRule) {
                this.BaseRule = rule;
            }

            this.uiService.Set(ui => {
                ui.Header.Title = "Endpoint Rule";
                ui.Header.SubTitle = rule.Name
                ui.Header.Icon = "edit"

                ui.Footer.Button1.Visible = true;
                ui.Footer.Button2.Visible = true;
            })

            this.form.patchValue(rule)
        })
    )

    form: FormGroup;

    @ViewChild(ActionsListComponent) actionsList: ActionsListComponent;
    @ViewChildren('addActionTemplate') addActionTemplate: QueryList<TemplateRef<any>>;

    constructor(private uiService: AppUIService, private route: ActivatedRoute, private rulesService: RulesService, private fb: FormBuilder, public overlay: Overlay, public viewContainerRef: ViewContainerRef) {

        this.uiService.Set(ui => {


            ui.Footer.Button1.OnClick = () => {
                this.save()
            }

            ui.Footer.Button2.Text = "Reset";
            ui.Footer.Button2.OnClick = () => {
                this.form.reset(this.BaseRule);
            }

        })
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

    ngOnInit() {

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

    onrightClick($event: MouseEvent) {
        $event.preventDefault();
    }

    initRestrictionsAccordion($event: ElementRef) {
        $($event.nativeElement).accordion()
    }


    GeneratePatchDocument() {
        var patchDocument = compare(this.BaseRule, this.form.value);
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

    private SetButtonStatus(rule: MiddlerRuleDto) {
        var patch = compare(this.BaseRule, rule)

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

    AddAction() {

        this.actionsList.AddAction();

    }

    private _baseRule: MiddlerRuleDto;
    private set BaseRule(rule: MiddlerRuleDto) {
        this._baseRule = JSON.parse(JSON.stringify(rule));

    }
    private get BaseRule() {
        return this._baseRule;
    }
}
