declare const Middler: middler.Scripting.Environment;

declare namespace middler.Scripting {

    export class Environment {
        Http: middler.Scripting.HttpCommand.Http;
        Task: any;
        Variables: middler.Scripting.Variables.VariableCommand;
    }
}