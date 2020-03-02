import { OverlayRef } from '@angular/cdk/overlay';

export class ActionEditModalOverlayRef {

    constructor(private overlayRef: OverlayRef, private onOk: ((value:any) => void)) { }

    close(): void {
        this.overlayRef.dispose();
    }

    ok(value: any) {
        if(this.onOk) {
            this.onOk(value);
        }
    }
}
