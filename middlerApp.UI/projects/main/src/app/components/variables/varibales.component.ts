import { Component, ChangeDetectionStrategy, ViewChild, TemplateRef, ViewContainerRef, ChangeDetectorRef } from "@angular/core";
import { SplitComponent, SplitAreaDirective } from 'angular-split';
import { OverlayRef, Overlay } from '@angular/cdk/overlay';
import { Subscription, fromEvent, BehaviorSubject } from 'rxjs';
import { TemplatePortal } from '@angular/cdk/portal';
import { filter, take, map } from 'rxjs/operators';
import { UIService } from '../main/ui.service';
import { VariablesService } from './varibales.service';
import { IActionMapping, TREE_ACTIONS, TreeNode, TreeComponent } from 'angular-tree-component';
import { StoreItem } from './store-item';
import { FolderTreeNode } from './folder-tree-node';

@Component({
    selector: 'variables',
    templateUrl: './varibales.component.html',
    styleUrls: ['./varibales.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class VariablesComponent {


    nodes$ = this.variablesService.rootNode$.pipe(
        map(node => node.Children)
    )

    private ItemsInPathSubject$ = new BehaviorSubject<Array<StoreItem>>([]);
    public ItemsInPath$ = this.ItemsInPathSubject$.asObservable();

    actionMapping: IActionMapping = {
        mouse: {
            click: (tree, node, $event) => {
                console.log(node)
                this.variablesService.SetSelectedFolder(node.data);
                this.variablesService.GetItemsInParent(node.data.Path).subscribe(items => this.ItemsInPathSubject$.next(items))
            },
            dblClick: (tree, node, $event) => {
                TREE_ACTIONS.TOGGLE_EXPANDED(tree, node, $event);
            },
            contextMenu: (tree, node, $event) => {
                this.variablesService.SetSelectedFolder(node.data);
                this.open($event, node)
                //this.onContextMenu($event, node.data, this.basicMenu);
            },
        }
    };

    options = {
        idField: "Path",
        displayField: "Name",
        childrenField: "Children",
        actionMapping: this.actionMapping,
        levelPadding: 10
    };

    @ViewChild("tree") private tree: TreeComponent;



    constructor(private ui: UIService, public overlay: Overlay, public viewContainerRef: ViewContainerRef, public variablesService: VariablesService, private cref: ChangeDetectorRef) {
        ui.Set(value => {
            value.Header.Title = "Global Variables"
            value.Header.Icon = 'folder open outline';
        })

    }


    @ViewChild('contextMenu') contextMenu: TemplateRef<any>;
    @ViewChild('addRuleTemplate') addRuleTemplate: TemplateRef<any>;
    overlayRef: OverlayRef | null;
    sub: Subscription;

    open($event: MouseEvent, node?: TreeNode) {

        if ($event.ctrlKey) {
            return;
        }

        $event.preventDefault();
        $event.stopPropagation();


        const clickTarget = event.target as HTMLElement;
        const isList = clickTarget.classList.contains('cdk-drop-list');

        // if (isList) {
        //     this.DeSelectAll();
        // }

        this.close();

        const { x, y } = $event;

        const positionStrategy = this.overlay.position()
            .flexibleConnectedTo({ x, y })
            .withPositions([
                {
                    originX: 'start',
                    originY: 'bottom',
                    overlayX: 'start',
                    overlayY: 'top',
                }
            ]);

        this.overlayRef = this.overlay.create({
            positionStrategy,
            scrollStrategy: this.overlay.scrollStrategies.close()
        });

        this.overlayRef.attach(new TemplatePortal(this.contextMenu, this.viewContainerRef, {
            $implicit: this.BuildContext(node)
        }));

        this.sub = fromEvent<MouseEvent>(document, 'click')
            .pipe(
                filter(event => {
                    const clickTarget = event.target as HTMLElement;
                    return !!this.overlayRef && !this.overlayRef.overlayElement.contains(clickTarget);
                }),
                take(1)
            ).subscribe(() => {
                this.close()

            })

    }

    private BuildContext(node: TreeNode) {

        return {
            Node: node,
            Folder: node?.data
        }

    }

    close() {
        this.sub && this.sub.unsubscribe();
        if (this.overlayRef) {
            this.overlayRef.dispose();
            this.overlayRef = null;
        }
    }

    clickFolderExpander(node: any, $event: MouseEvent) {
        $event.stopPropagation();
        $event.preventDefault();
        this.close();
        node.toggleExpanded();
    }

    public CreateSubFolder(node: TreeNode) {

        const nChild = new FolderTreeNode();
        nChild.Name = "New Folder";
        (<any>nChild).edit = "create";
        if (!node) {
            this.tree.treeModel.nodes = [nChild, ...this.tree.treeModel.nodes]
        } else {
            if (!node.data.Children) {
                node.children = []
                node.data.Children = []
            }
            node.data.Children = [nChild, ...node.data.Children];
        }


        this.tree.treeModel.update()
        this.close();
        this.cref.detectChanges();
        if(node) {
            this.tree.treeModel.getNodeById(node.id).expand()
        }
    }

    public saveName(node: TreeNode, nameinput: HTMLInputElement, $event) {
        if (!nameinput.value) {
            return;
        }
        switch (node.data.edit) {
            case 'create': {
                console.log("CreateFolder")
                node.data.Name = nameinput.value;
                this.variablesService.CreateDirectory(node.realParent?.data.Path || "", node.data.Name).subscribe();
                break;
            }
            case 'rename': {
                console.log("Rename Folder", node.data.Name, nameinput.value);
                node.data.edit = null;
                break;
            }
        }

    }

    public cancelCreateOrRenameFolder(node: TreeNode, $event: KeyboardEvent) {
        const method = node.data.edit;
        node.data.edit = "canceled";
        switch (method) {
            case 'create': {
                node.realParent.data.Children = node.realParent.data.Children.filter(c => c.Path != node.data.Path)
                this.tree.treeModel.update()
            }
            case 'rename': {

            }
        }

        node.data.edit = null;
        this.cref.detectChanges();
    }

    public RenameFolder(node: TreeNode) {

        node.data.edit = "rename";
        this.close();
        this.cref.detectChanges();
    }

    public DeleteDirectory(folder: FolderTreeNode) {
        this.variablesService.DeleteDirectory(folder.Path).subscribe();
        this.close();
    }

    public SelectIfCreateFolder(node:TreeNode, $event) {
        if(node.data.edit === 'create') {
            $event.target.select()
        }
        console.log(node.parent)
        node.parent.expand();
    }

}
