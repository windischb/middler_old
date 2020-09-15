import { Injectable } from "@angular/core";
import { HARRRConnection } from 'signalarrr';
import { HubConnectionState } from '@microsoft/signalr';
import { mergeAll } from 'rxjs/operators';
import { from, Subject } from 'rxjs';



@Injectable({
    providedIn: 'root'
})
export class MessageService {

    private harrrConnection: HARRRConnection;

    private _reconnectCallbacks: Array<() => void> = new Array<() => void>();

    private initializer = new Initializer();


    constructor() {

        this.harrrConnection = HARRRConnection.create(
            builder => builder
                .withUrl("/signalr/ui")
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: retryContext => {
                        
                        if (retryContext.elapsedMilliseconds < 600000) {
                            // If we've been reconnecting for less than 60 seconds so far,
                            // wait between 0 and 10 seconds before the next reconnect attempt.
                            return 1000;
                        } else {
                            // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
                            return null;
                        }
                    }
                })
        );


        this.harrrConnection.onReconnected(connectionId => {
            console.log("Reconnected", connectionId)
            if (this.harrrConnection.state == HubConnectionState.Connected) {
                this.InvokeOnEveryReconnectCallbacks();
            }
        })

        this.Start();
    }

    public Start() {
        this.harrrConnection.start().then(() => {
            this.initializer.SetInitialized();
            this.InvokeOnEveryReconnectCallbacks()
        })
    }

    public Invoke<T>(methodName: string, ...args: any[]) {


        return from(
            this.initializer.isInitialized.then(
                () => from(
                    this.harrrConnection.invoke<T>(methodName, ...args)
                )
            )
        ).pipe(mergeAll());
        //return from(this.harrrConnection.invoke<T>(methodName, args));
    }

    public Stream<T>(methodName: string, ...args: any[]) {
        return from(this.initializer.isInitialized.then(() => this._stream<T>(methodName, ...args))).pipe(mergeAll());
    }

    private _stream<T>(methodName: string, ...args: any[]) {
        const stream = this.harrrConnection.stream<T>(methodName, ...args);
        const subject = new Subject<T>();
        stream.subscribe(subject);
        return subject.asObservable();
    }


    public RunOnEveryReconnect(callback: () => void) {
        if (this._reconnectCallbacks.indexOf(callback) === -1) {
            this._reconnectCallbacks.push(callback);
        }
    }

    private InvokeOnEveryReconnectCallbacks() {
        console.log("InvokeOnEveryReconnectCallbacks")
        this._reconnectCallbacks.forEach(c => c());
    }

}


class Initializer {

    private initialized = false;
    private initializedSubject$ = new Subject<boolean>();
    public isInitialized = this.initializedSubject$.toPromise();

    public SetInitialized() {
        if (!this.initialized) {
            this.initialized = true;
            this.initializedSubject$.complete();
        }
    }
}
