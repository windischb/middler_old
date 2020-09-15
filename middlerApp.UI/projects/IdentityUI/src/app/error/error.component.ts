import { Component, ChangeDetectionStrategy } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IdpService } from '../shared/services/idp/idp.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { LoginInputModel } from '../shared/services/idp/models/login-input-model';
import { AuthenticationService } from '../shared/services/authentication-service';
import { BehaviorSubject } from 'rxjs';

@Component({
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class IdpErrorComponent {


    private errorSubject$ = new BehaviorSubject(null);
    public error$ = this.errorSubject$.asObservable();
    constructor(private fb: FormBuilder, private idpService: IdpService, private route: ActivatedRoute, private router: Router, private auth: AuthenticationService) { }



    ngOnInit(): void {
    

        this.route.queryParamMap.pipe(take(1)).subscribe(qmap => {

            let errorId: string = null;
            for (const key of qmap.keys) {
                if (key.toLowerCase() === 'errorid') {
                    errorId = qmap.get(key);
                }
            }
            console.log("errorId", errorId)
            this.idpService.GetErrorModel(errorId).subscribe(error => this.errorSubject$.next(error));

        });
    }

   

}