import { Injectable } from "@angular/core";
import { MessageService } from '../../shared/services/message.service';
import { tap, take, mergeMap } from 'rxjs/operators';
import { FolderTreeNode } from './folder-tree-node';
import { Subject, BehaviorSubject } from 'rxjs';
import { StoreItem } from './store-item';

@Injectable({
    providedIn: 'root'
})
export class VariablesService {

    private _rootNode = new FolderTreeNode();
    private rootNodeSubject$ = new BehaviorSubject<FolderTreeNode>(this._rootNode)
    rootNode$ = this.rootNodeSubject$.asObservable();

    SelectedFolder: any;
    SelectedFolder$: BehaviorSubject<any> = new BehaviorSubject<any>(null);


    set rootNode(value: FolderTreeNode) {
        this.rootNodeSubject$.next(value);
    }

    get rootNode() {
        return this._rootNode;
    }


    constructor(private messages: MessageService) {
        this.messages.RunOnEveryReconnect(() => this.GetFolderTree().subscribe())

        this.messages.Stream<any>("Variables.Subscribe").pipe(
            tap(s => console.log(s)),
            mergeMap(item => this.GetFolderTree())
        ).subscribe()

        this.GetFolderTree().subscribe();
    }

    public GetFolderTree() {

        return this.messages
            .Invoke<FolderTreeNode>("Variables.GetFolderTree")
            .pipe(
                take(1),
                tap(node => {
                    //console.log(node)
                    this.rootNode = node;
                })
            );
    }

    public GetItemsInParent(parent: string) {
        return this.messages.Invoke<Array<StoreItem>>("Variables.GetItemsInPath", parent)
        .pipe(
            take(1)
        )
    }

    public CreateDirectory(parent: string, name: string) {
        return this.messages.Invoke("Variables.CreateDirectory", parent, name)
        .pipe(
            take(1)
        )
    }

    public DeleteDirectory(path: string) {
        return this.messages.Invoke("Variables.DeleteDirectory", path)
        .pipe(
            take(1)
        )
    }

    public SetSelectedFolder(folder: any) {
        this.SelectedFolder$.next(folder);
        this.SelectedFolder = folder;
    }


}
