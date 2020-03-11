import { MiddlerAction } from '../../models/middler-action';

export class ScriptParameters {
    Language: "Javascript" | "Powershell" = null;
    SourceCode: string;
}

export class ScriptAction extends MiddlerAction<ScriptParameters> {

    ActionType = "Script"
    Parameters = new ScriptParameters()
}
