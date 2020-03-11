import { TemplateRef } from '@angular/core';

export class UIContext {


    Header = new UIHEader();
    Footer = new UIFooter();

    FooterTemplate: TemplateRef<any>
}

class UIHEader {

    Title: string;
    SubTitle: string;
    Icon: string;

    Outlet: TemplateRef<any>;
}

class UIFooter {

    Button1 = new UIButton();
    Button2 = new UIButton();
    Button3 = new UIButton();

    Outlet: TemplateRef<any>;
}

export class UIButton {

    public Text: string;
    public Disabled: boolean;
    public Loading: boolean;
    public Visible: boolean = false;

    public OnClick(action: () => void) {
        action()
    }
}
