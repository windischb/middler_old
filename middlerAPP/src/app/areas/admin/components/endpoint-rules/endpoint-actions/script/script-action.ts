import { EndpointAction } from '../../models/endpoint-action';


export class ScriptParameters {
    Language: "Javascript" | "Typescript" | "Powershell" = "Typescript";
    SourceCode: string;
}

export class ScriptAction extends EndpointAction<ScriptParameters> {

    ActionType = "Script"
    Parameters = new ScriptParameters()
}
