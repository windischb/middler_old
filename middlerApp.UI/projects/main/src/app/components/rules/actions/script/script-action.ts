import { MiddlerAction } from '../../models/middler-action';

export class ScriptParameters {
    Language: "Javascript" | "Typescript" | "Powershell" = "Typescript";
    SourceCode: string;
}

export class ScriptAction extends MiddlerAction<ScriptParameters> {

    ActionType = "Script"
    Parameters = new ScriptParameters()
}
