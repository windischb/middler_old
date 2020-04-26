﻿declare const Context: middler.Action.Scripting.Models.ScriptContext;

declare namespace middler.Action.Scripting.Models {

    export class ScriptContext {

        Request: IScriptContextRequest;
        Response: IScriptContextResponse;

        SendResponse(): void;
        ForwardBody(): string;
    }

    export interface IScriptContextRequest {
        HttpMethod: string;
        Uri: System.Uri;
        RouteData: SimpleDictionary<any>;
        Headers: SimpleDictionary<any>;
        QueryParameters: any;
        UserAgent: string;
        ClientIp: string;
        ProxyServers: Array<string>;

        GetBodyAsString(): string;
    }

    export interface IScriptContextResponse {
        StatusCode: number | null;
        Headers: SimpleDictionary<string>;

        SetBody(content: any): void
    }

    export class SimpleDictionary<T> {
        [key: string]: T;
        Count: number;
        Add(key: string, value: T): void;
        Remove(key: string): void;
        Clear(): void;
        IsEmpty(): boolean;
        GetKeys(): Array<string>;
        GetValues(): Array<T>;
        Map(action: ({ Key, Value }: { Key: string, Value: T }) => void): SimpleDictionary<T>;
        Filter(filter: ({ Key, Value }: { Key: string, Value: T }) => boolean): SimpleDictionary<T>;
        Merge(dict: SimpleDictionary<T>): SimpleDictionary<T>;
        static Merge<T>(dicts: SimpleDictionary<T>[]): SimpleDictionary<T>;
        

        constructor();
        constructor(value: {});

    }
}