import { Component, OnInit, ChangeDetectionStrategy, ContentChild, OnDestroy } from "@angular/core";
import { RulesService } from './rules.service';
import { List } from 'linqts';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { map, tap, takeUntil } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { ActionsListComponent } from './actions-list.component';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Subject } from 'rxjs';

@Component({
    templateUrl: './rules.component.html',
    styleUrls: ['./rules.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class RulesComponent implements OnInit, OnDestroy {

    public lastOrder: number = 0;
    rules$ = this.rulesService.MiddlerRules$;


    destroy$ = new Subject();
    constructor(private rulesService: RulesService, private router: Router, private route: ActivatedRoute) {

    }

    ngOnInit() {
        console.log("OnINIT")
        this.rulesService.MiddlerRules$.pipe(
            takeUntil(this.destroy$)
        ).subscribe()
    }

    ngOnDestroy() {
        console.log("Destroy")
        this.destroy$.next();
    }

    FormatActions(rule: Array<any>) {
        return rule.length;
    }

    editRule(id: string) {
        this.router.navigate([id], { relativeTo: this.route })
    }

    createRule() {
        this.router.navigate(["create"], { relativeTo: this.route })
    }


    drop(event: CdkDragDrop<string[]>) {
        const currentItem = this.rulesService.MiddlerRules[event.previousIndex];

        let direction = null;
        if (event.previousIndex < event.currentIndex) {
            direction = "forward"
        } else if (event.previousIndex > event.currentIndex) {
            direction = "back"
        }

        if (!direction) {
            return;
        }

        if (direction === 'forward') {
            if (event.currentIndex === this.rulesService.MiddlerRules.length - 1) {
                currentItem.Order = this.rulesService.GetNextLastOrder()
            } else {
                const beforeItem = this.rulesService.MiddlerRules[event.currentIndex]
                const nextItem = this.rulesService.MiddlerRules[event.currentIndex + 1]
                currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
            }
        } else if (direction === 'back') {
            if (event.currentIndex === 0) {
                currentItem.Order = this.rulesService.MiddlerRules[0].Order / 2
            } else {
                const beforeItem = this.rulesService.MiddlerRules[event.currentIndex -1]
                const nextItem = this.rulesService.MiddlerRules[event.currentIndex]
                currentItem.Order = ((nextItem.Order - beforeItem.Order) / 2) + beforeItem.Order
            }
        }


        moveItemInArray(this.rulesService.MiddlerRules, event.previousIndex, event.currentIndex);
        this.rulesService.MiddlerRules = [...this.rulesService.MiddlerRules]


    }

    public SaveOrder() {
        this.rulesService.UpdateRulesOrder();
    }
}
