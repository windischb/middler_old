export class LogOutModel {
    public LogoutId: string;
    public ShowLogoutPrompt = true;
    public Status: 'None' | 'Prompt' | 'LoggedOut';
    public PostLogoutRedirectUri: string;
    public ClientName: string;
    public SignOutIframeUrl: string;
    public AutomaticRedirectAfterSignOut = false;
}