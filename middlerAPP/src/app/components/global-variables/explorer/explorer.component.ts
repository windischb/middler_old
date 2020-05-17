import { Component, ChangeDetectionStrategy, ViewChild, TemplateRef, ViewContainerRef, ChangeDetectorRef } from "@angular/core";
import { map, filter, tap } from 'rxjs/operators';
import { VariablesService } from '../services/variables.service';
import { IActionMapping, TREE_ACTIONS, TreeNode, TreeComponent } from 'angular-tree-component';
import { FolderTreeNode } from '../models/folder-tree-node';

import { DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';
import { AppUIService } from '@services';
import { NzContextMenuService, NzDropdownMenuComponent } from 'ng-zorro-antd/dropdown';
import { NzFormatEmitEvent, NzTreeNodeOptions } from 'ng-zorro-antd/tree';

@Component({
    selector: 'global-variables-explorer',
    templateUrl: './explorer.component.html',
    styleUrls: ['./explorer.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class GlobalVariablesExplorerComponent {


    nodes$ = this.variablesService.rootNode$.pipe(
        map(node => node.Children)
    )

    get FolderTreeState() {
        return this.variablesService.FolderTreeState;
    }

    set FolderTreeState(value: any) {
        this.variablesService.FolderTreeState = value;
    }

    public ItemsInPath$ = this.variablesService.SelectedFolderItems$;

    public SelectedFolderPath$ = this.variablesService.SelectedFolder$.pipe(
        filter(s => !!s),
        map(s => s.Path)
    );

    actionMapping: IActionMapping = {
        mouse: {
            click: (tree, node, $event) => {
                this.variablesService.SetSelectedFolder(node.data);
                this.variablesService.GetVariablesInSelectedFolder().subscribe()
            },
            dblClick: (tree, node, $event) => {
                TREE_ACTIONS.TOGGLE_EXPANDED(tree, node, $event);
            },
            contextMenu: (tree, node, $event: MouseEvent) => {
                $event.preventDefault();
                $event.stopPropagation();
                this.variablesService.SetSelectedFolder(node.data);
                this.openFolderContextMenu($event, node)
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
    @ViewChild('folderContextMenu') folderContextMenu: TemplateRef<any>;


    private FolderContextMenu: IOverlayHandle;

    constructor(
        private ui: AppUIService,
        public viewContainerRef: ViewContainerRef,
        public variablesService: VariablesService,
        private cref: ChangeDetectorRef,
        private overlay: DoobOverlayService,
        private nzContextMenuService: NzContextMenuService) {

    }


    private NormalizeTreeNode(node: FolderTreeNode) {

        let n: NzTreeNodeOptions = {
            ...node,
            key: node.Path,
            title: node.Name,
            children: null
        }

        if(node.Children) {
            n.children = node.Children.map(c => this.NormalizeTreeNode(c));
        } else {
            n.isLeaf = true
        }

        return n;
    }

    clickTreeNode(event: NzFormatEmitEvent) {
        console.log(event)
        this.variablesService.SetSelectedFolder(event.node.origin as any);
        this.variablesService.GetVariablesInSelectedFolder().subscribe()
    }

    openFolderContextMenu($event: MouseEvent, node?: TreeNode) {
        this.FolderContextMenu?.Close();
        this.FolderContextMenu = this.overlay.OpenContextMenu($event, this.folderContextMenu, this.viewContainerRef, this.BuildContext(node))
    }

    private BuildContext(node: TreeNode) {
        return {
            Node: node,
            Folder: node?.data
        }
    }

    clickFolderExpander(node: any, $event: MouseEvent) {
        $event.stopPropagation();
        $event.preventDefault();
        this.CloseContextMenu();
        node.toggleExpanded();
    }

    private CloseContextMenu() {
        this.FolderContextMenu?.Close();
    }

    public CreateSubFolder(node: TreeNode) {

        const nChild = new FolderTreeNode();
        nChild.Name = "NewFolder";
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
        this.CloseContextMenu();
        this.cref.detectChanges();
        if (node) {
            this.tree.treeModel.getNodeById(node.id).expand()
        }
    }

    public saveName(node: TreeNode, nameinput: HTMLInputElement, $event) {
        if (!nameinput.value) {
            return;
        }
        switch (node.data.edit) {
            case 'create': {
                node.data.Name = nameinput.value;
                this.variablesService.NewFolder(`${node.realParent?.data.Parent || ""}/${node.realParent?.data.Name || ""}`, node.data.Name).subscribe();
                break;
            }
            case 'rename': {
                console.log(node.data.Parent, node.data.Name, nameinput.value)
                this.variablesService.RenameFolder(node.data.Parent, node.data.Name, nameinput.value).subscribe();
                node.data.Name = nameinput.value;
                node.data.edit = null;
                break;
            }
        }
        node.data.edit = null;
        this.cref.detectChanges();
    }

    public cancelCreateOrRenameFolder(node: TreeNode, $event: KeyboardEvent) {

        switch (node.data.edit) {
            case 'create': {
                if (!node.realParent) {
                    this.tree.treeModel.nodes = this.tree.treeModel.nodes.filter(c => c.Path != node.data.Path)
                } else {
                    node.realParent.data.Children = node.realParent.data.Children.filter(c => c.Path != node.data.Path)
                }

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
        this.CloseContextMenu();
        this.cref.detectChanges();
    }

    public DeleteDirectory(folder: FolderTreeNode) {
        this.variablesService.RemoveFolder(folder.Parent, folder.Name).subscribe();
        this.CloseContextMenu();
    }

    public SelectIfCreateFolder(node: TreeNode, $event) {
        if (node.data.edit === 'create') {
            $event.target.select()
        }
        node.parent.expand();
    }

    contextMenu($event: MouseEvent, menu: NzDropdownMenuComponent): void {
        this.nzContextMenuService.create($event, menu);
    }

    closeMenu(): void {
        this.nzContextMenuService.close();
    }

    nzEvent(event: NzFormatEmitEvent): void {
        console.log(event);
    }

}
