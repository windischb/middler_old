import { ConsentViewModel } from './consent-view-model';

export class ProcessConsentResult {
        public IsRedirect: boolean;
        public RedirectUri: string;
        public ClientId: string;

        public ShowView: boolean;
        public ViewModel: ConsentViewModel;

        public HasValidationError: boolean;
        public ValidationError: string;
    }
