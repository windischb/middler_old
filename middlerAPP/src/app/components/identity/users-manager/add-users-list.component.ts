import { Component, Input, Output, EventEmitter, OnInit } from "@angular/core";

import { IDPService } from '../identity.service';
import { combineLatest, Subject, BehaviorSubject, Observable, merge, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MUserListDto, MUserListDtoSortByName } from '../models/m-user-list-dto';
import { IdentityUsersQuery } from 'src/app/identity-users.store';

@Component({
    selector: 'add-users-list',
    templateUrl: './add-users-list.component.html',
    styleUrls: ['./add-users-list.component.scss']
})
export class AddUsersListComponent implements OnInit {

    private selectedUsersSubject$ = new BehaviorSubject<Array<MUserListDto>>([]);
    @Input()
    set selectedUsers(value: Array<MUserListDto>) {
        this.selectedUsersSubject$.next(value);
    }

    @Output() selectedUsersChanged = new EventEmitter<Array<MUserListDto>>()

    @Output()closeSidebar = new EventEmitter();

    form: FormGroup = this.fb.group({
        Filter: [null]
    })

    filter$ = merge(of(null), this.form.get("Filter").valueChanges)

    availableUsers$ = combineLatest(this.identityService.GetAllUsers(), this.selectedUsersSubject$, this.filter$).pipe(
        map(([available, selected, filter]) => {

            
            const deleted = available.filter(r => r.Deleted).map(r => r.Id);

            const selectedDeleted = selected.filter(r => deleted.includes(r.Id));
            if(selectedDeleted && selectedDeleted.length > 0) {
                const nSelected = selected.map(s => ({
                        ...s,
                        Deleted: selectedDeleted.includes(s)
                    })
                )
                this.selectedUsersChanged.next(nSelected);
            }

            let filtered = available;
            if (filter) {
                const reg = new RegExp(`${filter}`, "i")
                filtered = filtered.filter(r => {
                    return r.UserName.match(reg) || r.Email.match(reg) || r.Firstname.match(reg) || r.Lastname.match(reg)
                })
            }
            return filtered.sort(MUserListDtoSortByName).map(r => {
                return {
                    ...r,
                    Selected: selected.map(r => r.Id).includes(r.Id)
                }
            })
        })
    )



    constructor(
        private identityUsersQuery: IdentityUsersQuery,
        private identityService: IDPService,
        private fb: FormBuilder,

    ) {



    }

    ngOnInit() {

    }


    ToggleUser(role) {

        let nUsers = [];
        if (role.Selected) {
            nUsers = this.selectedUsersSubject$.getValue().filter(r => r.Id != role.Id);
        } else {
            nUsers = [...this.selectedUsersSubject$.getValue(), role];
        }
        this.selectedUsersChanged.next(nUsers);

    }

    Close() {
        this.closeSidebar.next()
    }
}