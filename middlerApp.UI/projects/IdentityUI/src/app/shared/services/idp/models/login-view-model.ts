import { LoginInputModel } from './login-input-model';

export class LoginViewModel extends LoginInputModel {
    public AllowRememberLogin: boolean;
    public EnableLocalLogin: boolean;

    public ExternalProviders: any;
    public LocalProviders: any;
    public DefaultProvider: string;

}
