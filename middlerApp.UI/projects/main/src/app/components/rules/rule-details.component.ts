import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from '@angular/router';
import { RulesService } from './rules.service';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { Observable, Subject } from 'rxjs';
import { map, mergeAll, tap } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
    templateUrl: './rule-details.component.html'
})
export class RuleDetailsComponent implements OnInit {


    public ruleSubject$: Subject<MiddlerRuleDto> = new Subject<MiddlerRuleDto>();
    public rule: MiddlerRuleDto;

    public rule$ = this.route.paramMap.pipe(
        map(paramMap => {
            const id = paramMap.get('id')
            return this.rulesService.Get(id);
        }),
        mergeAll(),
        tap(rule => {
            console.log(rule)
             this.form.patchValue(rule)
        })
    )

    form: FormGroup;

    constructor(private route: ActivatedRoute, private rulesService: RulesService, private fb: FormBuilder) {

        this.form = this.fb.group({
            Name: [],
            Scheme: [],
            Hostname: [],
            Path: []
        })
    }



    ngOnInit() {


    }

}
