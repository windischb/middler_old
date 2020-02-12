import { Component, OnInit } from "@angular/core";
import { RulesService } from './rules.service';
import { List } from 'linqts';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { map, tap } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './rules.component.html'
})
export class RulesComponent implements OnInit {

    public lastOrder: number = 0;
    rules$ = this.rulesService.MiddlerRules$.pipe(
        tap(rules => {
            if(!rules || rules.length == 0) {
                return []
            }
            var list = new List<MiddlerRuleDto>(rules).OrderBy(key => key.Order);
            this.lastOrder = list.Last().Order;
        })
    );

    constructor(private rulesService: RulesService, private router: Router, private route: ActivatedRoute) {

    }

    ngOnInit() {

    }

    FormatActions(rule: Array<any>) {
        return rule.length;
    }

    editRule(id: string) {
        this.router.navigate([id], {relativeTo: this.route})
    }
}
