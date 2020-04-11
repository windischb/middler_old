import { Component, ChangeDetectionStrategy, ViewChild, TemplateRef, ViewContainerRef, ChangeDetectorRef } from "@angular/core";
import { map, filter } from 'rxjs/operators';
import { VariablesService } from '../../services/variables.service';
import { IActionMapping, TREE_ACTIONS, TreeNode, TreeComponent } from 'angular-tree-component';
import { FolderTreeNode } from '../../models/folder-tree-node';
import { AppUIService } from 'app-services';
import { DoobOverlayService, IOverlayHandle } from '@doob-ng/cdk-helper';

@Component({
    templateUrl: './variables-explorer.component.html',
    styleUrls: ['./variables-explorer.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class VariablesExplorerComponent {


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
                this.variablesService.GetVariableInfosInSelectedFolder().subscribe()
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
        private overlay: DoobOverlayService) {

        ui.Set(value => {
            value.Header.Title = "Global Variables"
            value.Header.Icon = 'folder open outline';
            value.Content.Container = false;
            value.Footer.Show = false;
        })

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
                this.variablesService.NewFolder(node.realParent?.data.Path || "", node.data.Name).subscribe();
                break;
            }
            case 'rename': {
                node.data.Name = nameinput.value;
                this.variablesService.RenameFolder(node.data.Path, node.data.Name).subscribe();
                node.data.edit = null;
                break;
            }
        }

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
        this.variablesService.RemoveFolder(folder.Path).subscribe();
        this.CloseContextMenu();
    }

    public SelectIfCreateFolder(node: TreeNode, $event) {
        if (node.data.edit === 'create') {
            $event.target.select()
        }
        node.parent.expand();
    }

}
