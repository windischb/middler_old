// import { Input, Component } from "@angular/core";

// @Component({
//     selector: 'db-icon',
//     template: `
//     <i *ngIf="type == 'ant'" nz-icon [nzType]="nzIcon" [nzTheme]="theme" style="width:inherit" [nzRotate]="rotate"></i>
//     <fa-icon *ngIf="type == 'fa'" nz-icon [icon]="faIcon" style="width:inherit" [rotate]="rotate"></fa-icon>
//     `,
//     styles: [
//     `
//     :host {
//         display: inline-grid
//     }
//     `
//     ]
// })
// export class DoobIconComponent {


//     //private dbIconSubject$ = new BehaviorSubject<DbIcon>(new DbIcon());
//     //dbIcon$ = this.dbIconSubject$.asObservable();
//     @Input() type: "ant" | "fa" = "ant";
//     @Input() theme: string;
//     @Input() rotate: number = 0;

//     private _icon: string;

//     @Input()
//     set icon(value: string) {
        
//         if (!value) {
//             this._icon = null;
//             return;
//         }

//         if (value.startsWith('fa#')) {
//             this.type = "fa";
//             value = value.substring(3);

//             if (value.includes('|')) {
//                 this.theme = value.split('|')[0];
//                 this._icon = value.split('|')[1]
//             } else {
//                 this._icon = value;
//             }
            
//         } else {
//             this.type = "ant",
//             this._icon = value;
//         }
//     }

//     get icon() {
//         return this._icon;
//     }

//     get faIcon() {

//         if(this.theme) {
//             return [this.theme, this.icon]
//         } else {
//             return this.icon;
//         }
//     }

//     get nzIcon() {
//         return this.icon;
//     }
    
// }
