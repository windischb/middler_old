import { Injectable } from "@angular/core";
import { RulesService } from './components/rules/rules.service';
import { MessageService } from './shared/services/message.service';
import { MonacoLoaderService } from '@doob-ng/editor';

@Injectable({
    providedIn: 'root'
})
export class AppInitializeService {


    constructor(
        private rulesService: RulesService,
        private messageService: MessageService,
        private monaco: MonacoLoaderService
    ) {

    }
}
