import { Injectable } from "@angular/core";
import { MessageService } from '@services';
import { tap, take, mergeMap, switchMap } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { FolderTreeNode, VariableInfo, Variable } from '../models';


@Injectable({
    providedIn: 'root'
})
export class VariablesService {

    private rootNodeSubject$ = new BehaviorSubject<FolderTreeNode>(new FolderTreeNode())
    public rootNode$ = this.rootNodeSubject$.asObservable();

    private SelectedFolderSubject$: BehaviorSubject<FolderTreeNode> = new BehaviorSubject<FolderTreeNode>(null);
    public SelectedFolder$ = this.SelectedFolderSubject$.asObservable();

    private SelectedFolderItemsSubject$: BehaviorSubject<Array<VariableInfo>> = new BehaviorSubject<Array<VariableInfo>>(null);
    public SelectedFolderItems$ = this.SelectedFolderItemsSubject$.asObservable();

    FolderTreeState: any;

    constructor(private messages: MessageService) {
        this.messages.RunOnEveryReconnect(() => this.GetFolderTree().subscribe())

        this.messages.Stream<any>("Variables.Subscribe").pipe(
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
                    this.rootNodeSubject$.next(node);
                })
            );
    }

    public GetVariableInfosInSelectedFolder() {
        return this.messages.Invoke<Array<VariableInfo>>("Variables.GetVariableInfosInParent", this.SelectedFolderSubject$.getValue().Path)
            .pipe(
                take(1),
                tap(items => this.SelectedFolderItemsSubject$.next(items))
            )
    }

    public NewFolder(parent: string, name: string) {
        return this.messages.Invoke("Variables.NewFolder", parent, name)
            .pipe(
                take(1)
            )
    }

    public RenameFolder(path: string, name: string) {
        return this.messages.Invoke("Variables.RenameFolder", path, name)
            .pipe(
                take(1)
            )
    }

    public RemoveFolder(path: string) {
        return this.messages.Invoke("Variables.RemoveFolder", path)
            .pipe(
                take(1)
            )
    }

    public GetVariable(parent: string, name: string) {
        return this.messages.Invoke<Variable>("Variables.GetVariable", parent, name)
            .pipe(
                take(1)
            )
    }

    public UpdateVariableContent(path: string, content: string) {
        return this.messages.Invoke<Variable>("Variables.UpdateVariableContent", path, content)
            .pipe(
                take(1),
                switchMap(_ => this.GetVariableInfosInSelectedFolder())
            )
    }

    public CreateVariable(variable: Variable) {
        return this.messages.Invoke<Variable>("Variables.CreateVariable", variable)
            .pipe(
                take(1),
                switchMap(_ => this.GetVariableInfosInSelectedFolder())
            )
    }

    public RemoveVariable(path: string) {
        return this.messages.Invoke("Variables.RemoveVariable", path)
            .pipe(
                take(1),
                switchMap(_ => this.GetVariableInfosInSelectedFolder())
            )
    }

    public GetTypings() {
        return this.messages.Invoke<Array<{Key: string, Value: string}>>("Variables.GetTypings")
            .pipe(
                take(1)
            )
    }

    public SetSelectedFolder(folder: FolderTreeNode) {
        this.SelectedFolderSubject$.next(folder);
    }


}
