import { TemplateRef } from '@angular/core';

export class AppUIContext {
    Header = new AppUIHeader();
    Footer = new AppUIFooter();
    Content = new AppUIContent()
}

export class AppUIHeader {

    Title: string;
    SubTitle: string;
    Icon: string;

    Outlet: TemplateRef<any>;
}

class AppUIFooter {

    Show: boolean = true;

    Button1 = new AppUIButton();
    Button2 = new AppUIButton();
    Button3 = new AppUIButton();

    Outlet: TemplateRef<any>;
    UseTemplate: TemplateRef<any>
}

class AppUIButton {

    public Text: string;
    public Disabled: boolean;
    public Loading: boolean;
    public Visible: boolean = false;

    public OnClick(action: () => void) {
        action()
    }
}

class AppUIContent {
    Scrollable: boolean = false;
    Container: boolean = false;
}
