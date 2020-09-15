import { Component } from "@angular/core";
import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ICellRendererParams, IAfterGuiAttachedParams } from '@ag-grid-community/all-modules';
import { IMScopeDto } from '../models/m-scope-dto';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
    selector: 'scope-cell',
    templateUrl: './scope-grid-cell.component.html',
    styleUrls: ['./scope-grid-cell.component.scss']
})
export class ScopeGridCellComponent implements ICellRendererAngularComp {
    
    private scopeSubject$ = new BehaviorSubject<IMScopeDto>(null);
    scope$ = this.scopeSubject$.pipe(
        map(scope => {
            var ks = scope.Description?.replace(/\r/g, "").split(/\n/);
            return {
                ...scope,
                Description: ks && ks[0]
            }
        })
    );
    

    agInit(params: ICellRendererParams): void {
        this.scopeSubject$.next(params.data);
    }

    refresh(params: any): boolean {
        return false;
    }

    afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
        
    }

}