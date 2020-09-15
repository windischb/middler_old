import { ScopeViewModel } from './scope-view-model';
import { ConsentInputModel } from './consent-input-model';

export class ConsentViewModel extends ConsentInputModel {
    public ClientName: string;
    public ClientUrl: string;
    public ClientLogoUrl: string;
    public AllowRememberConsent = true;
    public IdentityScopes: Array<ScopeViewModel> = [];
    public ApiScopes: Array<ScopeViewModel> = [];
}
