import { Injectable } from "@angular/core";
// import { RulesService } from './components/rules/rules.service';
import { MessageService } from './shared/services/message.service';
import { AppUIService, AuthenticationService } from '@services';
import { EndpointRulesService } from './components/endpoint-rules/endpoint-rules.service';
import { MonacoLoaderService } from '@doob-ng/editor';
// import { MonacoLoaderService } from '@doob-ng/editor';

@Injectable({
    providedIn: 'root'
})
export class AppInitializeService {


    constructor(
        private rulesService: EndpointRulesService,
        private messageService: MessageService,
        private monaco: MonacoLoaderService,
        private authService: AuthenticationService
    ) {

       
    }
}
