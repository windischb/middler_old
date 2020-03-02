import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { CreateMiddlerRuleDto } from './models/create-middler-rule-dto';
import { of, BehaviorSubject } from 'rxjs';
import { tap, shareReplay } from 'rxjs/operators';
import { MessageService } from '../../shared/services/message.service';
import { compare } from 'fast-json-patch';

@Injectable({
    providedIn: 'root'
})
export class RulesService {


    private _middlerRules: Array<MiddlerRuleDto>;
    set MiddlerRules(value: Array<MiddlerRuleDto>) {
        this._middlerRules = (value || []).sort(this.SortRules);
        this.MiddlerRulesSubject$.next(this._middlerRules);
    }
    get MiddlerRules() {
        return this._middlerRules
    }

    private MiddlerRulesSubject$ = new BehaviorSubject<Array<MiddlerRuleDto>>(null);
    public MiddlerRules$ = this.MiddlerRulesSubject$.asObservable().pipe(shareReplay());

    private RulesOrder: {
        [key: string]: number
    } = {}

    constructor(private http: HttpClient, private message: MessageService) {

        this.message.RunOnEveryReconnect(() => this.GetAll().subscribe());


        this.message.Stream<any>("MiddlerRule.Subscribe").pipe(
            tap(item => console.log(item))
        ).subscribe()

    }

    public GetAll() {

        return this.message
            .Invoke<Array<MiddlerRuleDto>>("MiddlerRule.GetAll")
            .pipe(
                tap(rules => {
                    rules.forEach(r => this.RulesOrder[r.Id] = r.Order)
                    this.MiddlerRules = rules;
                })
            );
    }

    public Get(id: string) {
        const rule = this.MiddlerRules?.find(r => r.Id === id);
        if (rule) {
            return of(rule)
        } else {
            return this.message.Invoke<MiddlerRuleDto>("MiddlerRule.Get", id)
        }
    }

    public Add(rule: CreateMiddlerRuleDto) {
        return this.http.post(`/api/repo/litedb`, rule).pipe(
            tap(res => this.GetAll().subscribe())
        );
    }

    public Remove(id: string) {
        return this.http.delete(`/api/repo/litedb/${id}`).pipe(
            tap(res => this.GetAll().subscribe())
        );
    }

    public UpdatePartial(id: string, patchDocument: any) {

        const patchHeaders = new HttpHeaders({
            'Content-Type': 'application/json-patch+json',
            'Accept': 'application/json'
        })

        return this.http.patch<MiddlerRuleDto>(`/api/repo/litedb/${id}`, patchDocument, { headers: patchHeaders })
            .subscribe(rule => this.UpdatedMiddlerRule(rule));

    }

    private AddedMiddlerRule(value: MiddlerRuleDto) {
        this.MiddlerRules = [...this.MiddlerRules, value];
    }

    private UpdatedMiddlerRule(value: MiddlerRuleDto) {
        this.MiddlerRules = this.MiddlerRules.map(rule => {
            if (rule.Id == value.Id) {
                return value;
            } else {
                return rule;
            }
        });
    }

    private DeletedRule(value: MiddlerRuleDto | string) {
        let id: string;
        if (value instanceof MiddlerRuleDto) {
            id = value.Id;
        } else {
            id = value;
        }

        this.MiddlerRules = this.MiddlerRules.filter(rule => rule.Id == id);
    }

    private SortRules(a: MiddlerRuleDto, b: MiddlerRuleDto) {
        if (a.Order > b.Order) {
            return 1;
        } else if (a.Order < b.Order) {
            return -1;
        }
        return 0;
    }

    public GetNextLastOrder() {
        return Math.trunc(Math.max(...this.MiddlerRules.map(r => r.Order)) + 10);
    }

    public UpdateRulesOrder() {

        let _RulesOrder: {
            [key: string]: number
        } = {}
        this.MiddlerRules.forEach(r => _RulesOrder[r.Id] = r.Order);

        var patchDocument = compare(this.RulesOrder, _RulesOrder);

        console.log(patchDocument);

        const patchHeaders = new HttpHeaders({
            'Content-Type': 'application/json-patch+json',
            'Accept': 'application/json'
        })

        return this.http.patch<MiddlerRuleDto>(`/api/repo/litedb/order`, patchDocument, { headers: patchHeaders })
            .subscribe(_ => this.GetAll());
    }

}

