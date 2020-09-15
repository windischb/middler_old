
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { IconsProviderModule } from './icons-provider.module';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzTreeModule } from 'ng-zorro-antd/tree';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { DoobEditorModule } from '@doob-ng/editor';
import { DoobCoreModule } from '@doob-ng/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { DragDropModule } from "@angular/cdk/drag-drop";
import { DoobAntdExtensionsModule } from "@doob-ng/antd-extensions";
import { NgModule } from '@angular/core';
import { NzCalendarModule } from 'ng-zorro-antd/calendar';
import { CommonModule } from '@angular/common';
import { SimpleListComponent } from './components/simple-list/simple-list.component';
import { DoobGridModule } from '@doob-ng/grid';
import { DoobOverlayService } from '@doob-ng/cdk-helper';
import { OverlayModule } from '@angular/cdk/overlay';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
import { NzElementPatchModule } from 'ng-zorro-antd/core/element-patch'

const GlobalModules = [
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzInputNumberModule,
    NzCheckboxModule,
    NzSelectModule,
    NzDropDownModule,
    NzLayoutModule,
    NzMenuModule,
    NzTableModule,
    NzTabsModule,
    IconsProviderModule,
    NzCollapseModule,
    NzDatePickerModule,
    NzTreeModule,
    NzGridModule,
    NzToolTipModule,
    NzDrawerModule,
    NzCalendarModule,
    NzRadioModule,
    NzPopoverModule,
    NzElementPatchModule,

    DoobEditorModule,
    DoobCoreModule,
    DoobAntdExtensionsModule,
    DoobGridModule,

    FontAwesomeModule,

    DragDropModule,
    OverlayModule
]


@NgModule({
    imports: [
        CommonModule,
        ...GlobalModules
    ],
    declarations: [
        SimpleListComponent
    ],
    exports: [
        CommonModule,
        SimpleListComponent,
        ...GlobalModules
    ],
    providers: [
        DoobOverlayService
    ]
})
export class GlobalImportsModule {}