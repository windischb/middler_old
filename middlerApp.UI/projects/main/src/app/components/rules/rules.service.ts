import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MiddlerRuleDto } from './models/middler-rule-dto';
import { CreateMiddlerRuleDto } from './models/create-middler-rule-dto';
import { List } from 'linqts'
import { of, BehaviorSubject } from 'rxjs';
import { tap, shareReplay } from 'rxjs/operators';
import { MessageService } from '../../shared/services/message.service';

@Injectable({
    providedIn: 'root'
})
export class RulesService {

    private middlerRules: List<MiddlerRuleDto>;
    private MiddlerRulesSubject$ = new BehaviorSubject<Array<MiddlerRuleDto>>(null);
    public MiddlerRules$ = this.MiddlerRulesSubject$.asObservable().pipe(shareReplay());

    constructor(private http: HttpClient, private message: MessageService) {

        this.message.RunOnEveryReconnect(() => this.GetAll());


        this.message.Stream<any>("MiddlerRule.Subscribe").pipe(
            tap(item => console.log(item))
        ).subscribe()

    }

    public GetAll() {

        this.message
            .Invoke<Array<MiddlerRuleDto>>("MiddlerRule.GetAll")
            .subscribe(rules => {
                this.middlerRules = new List(rules);
                this.PublishRules();
            });
    }

    public StringToGuid(id: string) {
        this.message.Invoke<MiddlerRuleDto>("Test1.StringToGuid", id).subscribe(g => console.log(g))
    }

    public Get(id: string) {
        const rule = this.middlerRules?.FirstOrDefault(r => r.Id === id);
        if(rule) {
            return of(rule)
        } else {
            return this.message.Invoke<MiddlerRuleDto>("MiddlerRule.Get", id)
        }
    }

    public Add(rule: CreateMiddlerRuleDto) {
        return this.http.post(`/api/repo/litedb`, rule);
    }

    public Remove(id: string) {
        return this.http.delete(`/api/repo/litedb/${id}`)
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
        this.middlerRules.Add(value);
        this.PublishRules();
    }

    private UpdatedMiddlerRule(value: MiddlerRuleDto) {
        this.middlerRules = this.middlerRules.Select(rule => {
            if (rule.Id == value.Id) {
                return value;
            } else {
                return rule;
            }
        });
        this.PublishRules();
    }

    private DeletedRule(value: MiddlerRuleDto | string) {
        let id: string;
        if (value instanceof MiddlerRuleDto) {
            id = value.Id;
        } else {
            id = value;
        }

        this.middlerRules = this.middlerRules.Where(rule => rule.Id == id);
        this.PublishRules();
    }

    private PublishRules() {
        this.middlerRules = this.middlerRules?.OrderBy(r => r.Order);
        this.MiddlerRulesSubject$.next(this.middlerRules?.ToArray());
    }
}
