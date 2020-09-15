export class ConsentInputModel {
    public Button: string;
    public ScopesConsented: Array<string> = new Array<string>();
    public RememberConsent = false;
    public ReturnUrl: string;
}
