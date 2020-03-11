import { Injectable } from "@angular/core";
import { RulesService } from './components/rules/rules.service';

@Injectable({
    providedIn: 'root'
})
export class AppInitializeService {


    constructor(
        private rulesService: RulesService
    ) {

    }
}
