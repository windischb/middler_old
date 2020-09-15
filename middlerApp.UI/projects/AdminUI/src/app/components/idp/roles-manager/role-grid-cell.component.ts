import { Component } from "@angular/core";
import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ICellRendererParams, IAfterGuiAttachedParams } from '@ag-grid-community/all-modules';
import { MRoleDto } from '../models/m-role-dto';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
    selector: 'role-cell',
    templateUrl: './role-grid-cell.component.html',
    styleUrls: ['./role-grid-cell.component.scss']
})
export class RoleGridCellComponent implements ICellRendererAngularComp {
    
    private roleSubject$ = new BehaviorSubject<MRoleDto>(null);
    role$ = this.roleSubject$.pipe(
        map(role => {
            var ks = role.Description?.replace(/\r/g, "").split(/\n/);
            return {
                ...role,
                Description: ks && ks[0]
            }
        })
    );
    

    agInit(params: ICellRendererParams): void {
        this.roleSubject$.next(params.data);
    }

    refresh(params: any): boolean {
        return false;
    }

    afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
        
    }

}