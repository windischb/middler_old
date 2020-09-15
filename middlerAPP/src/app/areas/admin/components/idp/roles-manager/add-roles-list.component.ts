import { Component, Input, Output, EventEmitter, OnInit } from "@angular/core";
import { IdentityRolesQuery } from '../identity-roles.store';
import { IDPService } from '../idp.service';
import { MRoleDto, MRoleDtoSortByName } from '../models/m-role-dto';
import { combineLatest, Subject, BehaviorSubject, Observable, merge, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
    selector: 'add-roles-list',
    templateUrl: './add-roles-list.component.html',
    styleUrls: ['./add-roles-list.component.scss']
})
export class AddRolesListComponent implements OnInit {

    private selectedRolesSubject$ = new BehaviorSubject<Array<MRoleDto>>([]);
    @Input()
    set selectedRoles(value: Array<MRoleDto>) {
        this.selectedRolesSubject$.next(value);
    }

    @Output() selectedRolesChanged = new EventEmitter<Array<MRoleDto>>()

    @Output()closeSidebar = new EventEmitter();

    form: FormGroup = this.fb.group({
        Filter: [null]
    })

    filter$ = merge(of(null), this.form.get("Filter").valueChanges)

    availableRoles$ = combineLatest(this.identityService.GetAllRoles(), this.selectedRolesSubject$, this.filter$).pipe(
        map(([available, selected, filter]) => {

            
            const deleted = available.filter(r => r.Deleted).map(r => r.Id);

            const selectedDeleted = selected.filter(r => deleted.includes(r.Id));
            if(selectedDeleted && selectedDeleted.length > 0) {
                const nSelected = selected.map(s => ({
                        ...s,
                        Deleted: selectedDeleted.includes(s)
                    })
                )
                this.selectedRolesChanged.next(nSelected);
            }

            let filtered = available;
            if (filter) {
                const reg = new RegExp(`${filter}`, "i")
                filtered = filtered.filter(r => {
                    return r.Name.match(reg) || r.DisplayName.match(reg)
                })
            }
            return filtered.sort(MRoleDtoSortByName).map(r => {
                return {
                    ...r,
                    Selected: selected.map(r => r.Id).includes(r.Id)
                }
            })
        })
    )



    constructor(
        private identityRolesQuery: IdentityRolesQuery,
        private identityService: IDPService,
        private fb: FormBuilder,

    ) {



    }

    ngOnInit() {

    }


    ToggleRole(role) {

        let nRoles = [];
        if (role.Selected) {
            nRoles = this.selectedRolesSubject$.getValue().filter(r => r.Id != role.Id);
        } else {
            nRoles = [...this.selectedRolesSubject$.getValue(), role];
        }
        this.selectedRolesChanged.next(nRoles);

    }

    Close() {
        this.closeSidebar.next()
    }
}