import { Injectable, Inject, OnInit, Injector, ComponentRef, Type, TemplateRef, EmbeddedViewRef } from '@angular/core';
import { Overlay, OverlayConfig, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal, PortalInjector, TemplatePortal } from '@angular/cdk/portal';


import { MiddlerAction } from '../models/middler-action';
import { ActionEditModalOverlayRef } from './action-edit-modal-overlay-ref';
import { ActionEditModalComponent } from './action-edit-modal.component';
import { ACTION_DIALOG_DATA } from './action-edit-modal.tokens';

export interface ActionContext {
    Action: MiddlerAction;
    url: string;
}

interface ActionEditModalConfig {
    panelClass?: string;
    hasBackdrop?: boolean;
    backdropClass?: string;
    actionContext?: MiddlerAction;
    onOk?: ((value: any) => void);
}

const DEFAULT_CONFIG: ActionEditModalConfig = {
    hasBackdrop: true,
    backdropClass: 'dark-backdrop',
    panelClass: 'tm-file-preview-dialog-panel',
    actionContext: null,
    onOk: null
}

@Injectable()
export class ActionEditModalService {

    constructor(
        private injector: Injector,
        private overlay: Overlay) { }

    OpenComponent<T>(component: Type<T>, config: ActionEditModalConfig = {}) {
        // Override default configuration
        const dialogConfig = { ...DEFAULT_CONFIG, ...config };

        // Returns an OverlayRef which is a PortalHost
        const overlayRef = this.createOverlay(dialogConfig);

        // Instantiate remote control
        const dialogRef = new ActionEditModalOverlayRef(overlayRef, config.onOk);

        const overlayComponent = this.attachComponentToDialogContainer(overlayRef, dialogConfig, dialogRef, component);

        overlayRef.backdropClick().subscribe(_ => dialogRef.close());

        return dialogRef;
    }


    private createOverlay(config: ActionEditModalConfig) {
        const overlayConfig = this.getOverlayConfig(config);
        return this.overlay.create(overlayConfig);
    }

    private attachComponentToDialogContainer<T>(overlayRef: OverlayRef, config: ActionEditModalConfig, dialogRef: ActionEditModalOverlayRef, component: Type<T>) {
        const injector = this.createInjector(config, dialogRef);

        const containerPortal = new ComponentPortal(component, null, injector);
        const containerRef: ComponentRef<T> = overlayRef.attach(containerPortal);

        return containerRef.instance;
    }

    private attachTemplateToDialogContainer<T>(overlayRef: OverlayRef, config: ActionEditModalConfig, template: TemplateRef<any>) {

        const containerPortal = new TemplatePortal(template, null, {
            $implicit: config.actionContext
        });
        const containerRef: EmbeddedViewRef<T> = overlayRef.attach(containerPortal);


        return containerRef;
    }

    private createInjector(config: ActionEditModalConfig, dialogRef: ActionEditModalOverlayRef): PortalInjector {
        const injectionTokens = new WeakMap();

        injectionTokens.set(ActionEditModalOverlayRef, dialogRef);
        injectionTokens.set(ACTION_DIALOG_DATA, config.actionContext);

        return new PortalInjector(this.injector, injectionTokens);
    }

    private getOverlayConfig(config: ActionEditModalConfig): OverlayConfig {
        const positionStrategy = this.overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();

        const overlayConfig = new OverlayConfig({
            hasBackdrop: config.hasBackdrop,
            backdropClass: config.backdropClass,
            panelClass: config.panelClass,
            scrollStrategy: this.overlay.scrollStrategies.block(),
            positionStrategy
        });

        return overlayConfig;
    }
}
